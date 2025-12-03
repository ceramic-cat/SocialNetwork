using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;
using SocialNetwork.Entity.Models.Follow;
using SocialNetwork.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository.Repositories;



public class UserFollowsRepository : IFollowRepository
{
    private readonly SocialNetworkDbContext _db;

    public UserFollowsRepository(SocialNetworkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Guid followerId, Guid followeeId)
    {
        var userFollow = new UserFollows 
        {
            FollowerId = followerId , 
            FolloweeId = followeeId
        };

        _db.UserFollows.Add(userFollow);
        await _db.SaveChangesAsync();
        
    }
    public async Task DeleteAsync(Guid followerId, Guid followeeId)
    {
        var follow = await _db.UserFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

        if (follow is not null)
        {
            _db.UserFollows.Remove(follow);
            await _db.SaveChangesAsync();
        }

    }
    public async Task<bool> ExistsAsync(Guid followerId, Guid followeeId)
    { 
        return await _db.UserFollows
            .AnyAsync(i => i.FollowerId == followerId && i.FolloweeId == followeeId);

    }

}
