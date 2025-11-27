namespace SocialNetwork.Repository.Services
{
    public interface IPostService
    {
        Task<PostResult> CreatePostAsync(Guid senderId, Guid receiverId, string message);
    }
}
