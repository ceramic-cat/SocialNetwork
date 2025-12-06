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
        var directMessage = new DirectMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId ?? Guid.Empty,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _db.DirectMessages.Add(directMessage);
        await _db.SaveChangesAsync();
        return Result.Success();
    }
}

