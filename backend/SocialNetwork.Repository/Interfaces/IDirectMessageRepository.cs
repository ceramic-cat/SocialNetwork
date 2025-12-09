using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Interfaces;

public interface IDirectMessageRepository
{
    Task AddAsync(DirectMessage directMessage);
}
