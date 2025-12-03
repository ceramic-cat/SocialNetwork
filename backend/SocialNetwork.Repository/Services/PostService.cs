using SocialNetwork.Entity;
using SocialNetwork.Entity.Models;
using System;
using System.Threading.Tasks;

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

    public class PostService : IPostService
    {
        private readonly SocialNetworkDbContext _db;
        private const int MaxContentLength = 280;

        public PostService(SocialNetworkDbContext db)
        {
            _db = db;
        }

        public async Task<PostResult> CreatePostAsync(Guid senderId, Guid receiverId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return PostResult.Fail("Content cannot be empty.");
            }

            if (content.Length > MaxContentLength)
            {
                return PostResult.Fail($"Content cannot be longer than {MaxContentLength} characters.");
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            return PostResult.Ok();
        }
    }
}
