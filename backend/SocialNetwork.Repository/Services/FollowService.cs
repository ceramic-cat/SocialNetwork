using SocialNetwork.API.Models;
using SocialNetwork.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository.Services;


public interface IFollowsService
{
    Task<Result> FollowAsync(Guid follower, Guid followee);
    Task<Result> UnfollowAsync(Guid follower, Guid followee);
}
public class FollowService : IFollowsService
{
    private readonly IFollowRepository _repository;

    public FollowService(IFollowRepository repository)
    {
        _repository = repository;
    }

     public async Task<Result> FollowAsync(Guid follower, Guid followee)
    {
        if (follower == followee)
        {
            return Result.Failure("You can't follow yourself");
        }

        if(await _repository.ExistsAsync(follower, followee))
        {
            return Result.Failure("Already following this user");
        }

        try
        {
            await _repository.AddAsync(follower, followee);
            return Result.Success();
        }
        catch (Exception ex) { 
        return Result.Failure(ex.Message);
        }
    }

    public async Task<Result> UnfollowAsync(Guid follower, Guid followee)
    {
        if (false == await _repository.ExistsAsync(follower, followee))
        {
            return Result.Failure("Unable to unfollow that user");
        }

        return Result.Success();
    }
}

