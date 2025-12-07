using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Services;

public class DirectMessageService : IDirectMessageService
{
    private readonly SocialNetworkDbContext _db;
    private const int MaxContentLength = 280;

    public DirectMessageService(SocialNetworkDbContext db)
    {
        _db = db;
    }

    public async Task<Result> SendDirectMessageAsync(Guid senderId, Guid? receiverId, string content)
    {
        throw new NotImplementedException();
    }
}

