using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.API.Models
{
    public class CreateDirectMessageRequest
    {
        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        [StringLength(280, MinimumLength = 1)]
        [MaxLength(280)]
        public string Content { get; set; } = string.Empty;
    }
}
