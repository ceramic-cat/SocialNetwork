using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Errors;
using SocialNetwork.Repository.Interfaces;

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
        private readonly IPostRepository _postRepository;
        private const int MaxContentLength = 280;

        public PostService(SocialNetworkDbContext db, IPostRepository postRepository)
        {
            _db = db;
            _postRepository = postRepository;
        }

        public async Task<PostResult> CreatePostAsync(Guid senderId, Guid receiverId, string content)
        {
            if (senderId == Guid.Empty)
                return PostResult.Fail(PostErrors.SenderEmpty);

            if (receiverId == Guid.Empty)
                return PostResult.Fail(PostErrors.RecieverEmpty);

            if (string.IsNullOrWhiteSpace(content))
                return PostResult.Fail(PostErrors.ContentEmpty);

            if (content.Length > MaxContentLength)
                return PostResult.Fail(PostErrors.ContentTooLong);

            var senderExists = await _db.Users.AnyAsync(u => u.Id == senderId);
            if (!senderExists)
                return PostResult.Fail(PostErrors.SenderDoesNotExist);

            var receiverExists = await _db.Users.AnyAsync(u => u.Id == receiverId);
            if (!receiverExists)
                return PostResult.Fail(PostErrors.ReceiverDoesNotExist);

            var post = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _postRepository.AddPostAsync(post);

            return PostResult.Ok();
        }

        public async Task<PostResult> DeletePostAsync(Guid postId, Guid userId)
        {
            if (postId == Guid.Empty)
                return PostResult.Fail(PostErrors.PostIdEmpty);

            if (userId == Guid.Empty)
                return PostResult.Fail(PostErrors.InvalidUserId);

            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null)
                return PostResult.Fail(PostErrors.PostNotFound);

            if (post.SenderId != userId)
                return PostResult.Fail(PostErrors.NotAllowedToDelete);

            await _postRepository.DeletePostAsync(post);

            return PostResult.Ok();
        }
    }
}
