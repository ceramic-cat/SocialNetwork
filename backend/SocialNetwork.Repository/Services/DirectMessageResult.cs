namespace SocialNetwork.Repository.Services
{
    public class DirectMessageResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }

        public static DirectMessageResult Fail(string message)
            => new DirectMessageResult { Success = false, ErrorMessage = message };

        public static DirectMessageResult Ok()
            => new DirectMessageResult { Success = true };
    }
}

