namespace SocialNetwork.Repository.Services;

public interface IFollowRepository
{
    Task<bool> IsFollowingAsync(Guid followerId, Guid followeeId);
}