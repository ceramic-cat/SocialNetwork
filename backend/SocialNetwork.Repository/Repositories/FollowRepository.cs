using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository.Repositories;



public class FollowRepository : IFollowRepository
{
    private readonly SocialNetworkDbContext _db;

    public FollowRepository(SocialNetworkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Guid followerId, Guid followeeId)
    {
        var userFollow = new Follow
        {
            FollowerId = followerId,
            FolloweeId = followeeId
        };

        _db.Follows.Add(userFollow);
        await _db.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid followerId, Guid followeeId)
    {
        var follow = await _db.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

        if (follow is not null)
        {
            _db.Follows.Remove(follow);
            await _db.SaveChangesAsync();
        }
    }
    public async Task<bool> ExistsAsync(Guid followerId, Guid followeeId)
    {
        return await _db.Follows
            .AnyAsync(i => i.FollowerId == followerId && i.FolloweeId == followeeId);

    }
    public async Task<Guid[]> GetFollowsAsync(Guid follower)
    {
        return await _db.Follows
            .Where(f => f.FollowerId == follower)
            .Select(f => f.FolloweeId)
            .ToArrayAsync();
    }

    public async Task<FollowedUserDto[]> GetFollowsWithUserInfoAsync(Guid followerId)
    {
        return await _db.Follows
            .Where(f => f.FollowerId == followerId)
            .Join(
                _db.Users,
                f => f.FolloweeId,
                u => u.Id,
                (f, u) => new FollowedUserDto { Id = u.Id, Username = u.Username }
            )
            .ToArrayAsync();
    }
}
