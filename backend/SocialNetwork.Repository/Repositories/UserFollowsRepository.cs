using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository.Repositories;



public class UserFollowsRepository : IUserFollowsRepository
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
    public Task DeleteAsync(Guid followerId, Guid followeeId) => throw new NotImplementedException();
    public async Task<bool> ExistsAsync(Guid followerId, Guid followeeId)
    { 
        return await _db.UserFollows
            .AnyAsync(i => i.FollowerId == followerId && i.FolloweeId == followeeId);

    }

}
