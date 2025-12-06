using Microsoft.EntityFrameworkCore;
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

        private PostResult(bool success, string? errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static PostResult Fail(string message)
            => new PostResult(false, message);

        public static PostResult Ok()
            => new PostResult(true);
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
            if (senderId == Guid.Empty)
            {
                return PostResult.Fail("SenderId cannot be empty.");
            }
            if (receiverId == Guid.Empty)
            {
                return PostResult.Fail("ReceiverId cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return PostResult.Fail("Content cannot be empty.");
            }

            if (content.Length > MaxContentLength)
            {
                return PostResult.Fail($"Content cannot be longer than {MaxContentLength} characters.");
            }

            var senderExists = await _db.Users.AnyAsync(u => u.Id == senderId);
            if (!senderExists)
            {
                return PostResult.Fail("Sender does not exist.");
            }

            var receiverExists = await _db.Users.AnyAsync(u => u.Id == receiverId);
            if (!receiverExists)
            {
                return PostResult.Fail("Receiver does not exist.");
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

        public async Task<PostResult> DeletePostAsync(Guid postId, Guid userId)
        {
            if (postId == Guid.Empty)
            {
                return PostResult.Fail("PostId cannot be empty.");
            }

            if (userId == Guid.Empty)
            {
                return PostResult.Fail("RequesterId cannot be empty.");
            }

            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return PostResult.Fail("Post not found.");
            }

            if (post.SenderId != userId)
            {
                return PostResult.Fail("You are not allowed to delete this post.");
            }

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();

            return PostResult.Ok();
        }

    }
}
