using SocialNetwork.API.Models;

namespace SocialNetwork.Repository.Services;

public interface IDirectMessageService
{
    Task<Result> SendDirectMessageAsync(Guid senderId, Guid? receiverId, string content);
}

