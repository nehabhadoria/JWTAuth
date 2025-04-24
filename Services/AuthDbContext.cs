using JWTAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.Services
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbUser> Users { get; set; }
    }
}
