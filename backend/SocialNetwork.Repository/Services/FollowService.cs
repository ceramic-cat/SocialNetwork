using SocialNetwork.API.Models;
using SocialNetwork.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository.Services;


public interface IFollowService
{
    public Task<Result> FollowUserAsync(Guid follower, Guid followee);
}

public class FollowService : IFollowService
{
    private readonly IFollowRepository _repository;

    public FollowService(IFollowRepository repository)
    {
        _repository = repository;
    }

    public Task<Result> FollowUserAsync(Guid follower, Guid followee) => throw new NotImplementedException();
}
