using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Services;

public interface IDirectMessageService
{
    Task<Result> SendDirectMessageAsync(Guid senderId, Guid receiverId, string content);
    Task<List<DirectMessage>> GetMessagesByUserIdAsync(Guid userId);
}

