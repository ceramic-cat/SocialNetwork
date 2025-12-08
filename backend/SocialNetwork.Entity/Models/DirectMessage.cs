using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Entity.Models
{
    [Table("directMessages")]
    public class DirectMessage
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("senderId")]
        public Guid SenderId { get; set; }

        [Column("receiverId")]
        public Guid ReceiverId { get; set; }

        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

