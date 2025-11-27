using System;
using System.Threading.Tasks;
using SocialNetwork.Entity;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Services
{
    public class PostResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }

        public static PostResult Fail(string message)
            => new PostResult { Success = false, ErrorMessage = message };

        public static PostResult Ok()
            => new PostResult { Success = true };
    }

    public class PostService
    {
        private readonly IPostRepository _postRepository;
        private const int MaxMessageLength = 280;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<PostResult> CreatePostAsync(Guid senderId, Guid receiverId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return PostResult.Fail("Message cannot be empty.");
            }

            if (message.Length > MaxMessageLength)
            {
                return PostResult.Fail($"Message cannot be longer than {MaxMessageLength} characters.");
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _postRepository.AddAsync(post);

            return PostResult.Ok();
        }
    }
}
