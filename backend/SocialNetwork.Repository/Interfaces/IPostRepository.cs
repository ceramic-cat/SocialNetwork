using SocialNetwork.Entity;

namespace SocialNetwork.Repository.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
    }
}
