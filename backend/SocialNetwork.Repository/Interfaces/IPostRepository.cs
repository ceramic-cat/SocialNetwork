using SocialNetwork.Entity.Models;
namespace SocialNetwork.Repository.Interfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetPostsByUserIdAsync(Guid userId);
        Task<List<Post>> GetPostsByUserIdsAsync(Guid[] userIds);
    }
}
