using System;
using System.Threading.Tasks;

namespace SocialNetwork.Repository.Services
{
    public interface IPostService
    {
        Task<PostResult> CreatePostAsync(Guid senderId, Guid receiverId, string content);
        Task<PostResult> DeletePostAsync(Guid postId, Guid userId);
    }
}
