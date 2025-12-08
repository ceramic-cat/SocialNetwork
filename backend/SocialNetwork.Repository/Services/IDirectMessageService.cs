namespace SocialNetwork.Repository.Services
{
    public interface IDirectMessageService
    {
        Task<DirectMessageResult> SendDirectMessageAsync(Guid senderId, Guid receiverId, string content);
    }
}
