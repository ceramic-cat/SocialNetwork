using SocialNetwork.API.Models;
using SocialNetwork.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository.Services;


public interface IUserFollowsService
{
    Task<Result> FollowAsync(Guid follower, Guid followee);
    Task<Result> UnfollowAsync(Guid follower, Guid followee);
}
public class UserFollowsService : IUserFollowsService
{
    private readonly IUserFollowsRepository _repository;

    public UserFollowsService(IUserFollowsRepository repository)
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

        return Result.Success();

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

