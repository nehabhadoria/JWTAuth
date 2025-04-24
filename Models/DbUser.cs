using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuth.Models
{
    [Table("TBL_PL_MT_USER")]
    public class DbUser
    {
        [Key]
        public int Id { get; set; }  // replace with actual PK column name if different

        public string? Email { get; set; }
    }
}
