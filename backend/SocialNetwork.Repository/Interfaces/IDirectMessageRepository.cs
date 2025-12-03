using SocialNetwork.Entity;

namespace SocialNetwork.Repository.Interfaces
{
    public interface IDirectMessageRepository
    {
        Task AddAsync(DirectMessage directMessage);
    }
}

