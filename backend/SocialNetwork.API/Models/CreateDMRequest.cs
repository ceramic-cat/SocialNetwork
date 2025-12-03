namespace SocialNetwork.API.Models
{
    public class CreateDMRequest
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

