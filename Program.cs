using System.Text;
using JWTAuth.helpers;
using JWTAuth.Models;
using JWTAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load JWT config safely
var jwtKey = builder.Configuration["Jwt:Key"] ?? "fallback-key";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "fallback-issuer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "fallback-audience";

// ✅ Register DbContext for user lookup
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Register AuthService
builder.Services.AddTransient<AuthService>();

// ✅ Configure CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:59771", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ✅ (Optional) Authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("tech", policy => policy.RequireRole("developer"));
});

var app = builder.Build();

// ✅ Apply CORS middleware BEFORE auth
app.UseCors("AllowAngularDev");

app.UseAuthentication();
app.UseAuthorization();

// ✅ POST /login: Return token if email exists
app.MapPost("/login", async (LoginRequest req, AuthService service) =>
{
    var token = await service.LoginAsync(req.Email);

    if (token == null)
        return Results.Unauthorized();

    return Results.Ok(new { token });
});

// ✅ Protected test routes
app.MapGet("/test", () => "OK!").RequireAuthorization();
app.MapGet("/test/tech", () => "tech OK!").RequireAuthorization("tech");

app.Run();
