using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Entity.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("username")]
        public string Username { get; set; } = default!;

        [Column("email")]
        public string Email { get; set; } = default!;

        [Column("password")]
        public string Password { get; set; } = default!;

        [Column("created")]
        public string Created { get; set; } = default!;
    }
}