using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Interfaces;

public interface IFollowRepository
{
    Task<bool> ExistsAsync(Guid followerId, Guid followeeId);
    Task AddAsync(Guid followerId, Guid followeeId);
    Task DeleteAsync(Guid followerId, Guid followeeId);
    Task<Guid[]> GetFollowsAsync(Guid follower);
    Task<FollowedUserDto[]> GetFollowsWithUserInfoAsync(Guid followerId);
    Task<FollowerUserDto[]> GetFollowersWithUserInfoAsync(Guid followeeId);
    Task<int> GetFollowersCountAsync(Guid userId);
    Task<int> GetFollowingCountAsync(Guid userId);
}