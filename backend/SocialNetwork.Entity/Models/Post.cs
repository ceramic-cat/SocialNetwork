namespace SocialNetwork.Entity.Models
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}
