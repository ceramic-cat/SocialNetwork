using SocialNetwork.API.Models;
using SocialNetwork.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository.Services;


public interface IFollowService
{
    Task<Result> FollowUserAsync(Guid follower, Guid followee);
}

public class FollowService : IFollowService
{
    private readonly IFollowRepository _repository;

    public FollowService(IFollowRepository repository)
    {
        _repository = repository;
    }

     public async Task<Result> FollowUserAsync(Guid follower, Guid followee)
    {
        if (follower == followee)
        {
            return Result.Failure("You can't follow yourself");
        }

        if(await _repository.IsFollowingAsync(follower, followee))
        {
            return Result.Failure("Already following this user");
        }

        return Result.Success();

    }
        
        
        
}
