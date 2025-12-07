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
        if (senderId == Guid.Empty)
        {
            return Result.Failure("SenderId cannot be empty.");
        }

        if (receiverId == null || receiverId == Guid.Empty)
        {
            return Result.Failure("ReceiverId cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result.Failure("Content cannot be empty.");
        }

        if (content.Length > MaxContentLength)
        {
            return Result.Failure($"Content cannot be longer than {MaxContentLength} characters.");
        }

        var senderExists = await _db.Users.AnyAsync(u => u.Id == senderId);
        if (!senderExists)
        {
            return Result.Failure("Sender does not exist.");
        }

        var receiverExists = await _db.Users.AnyAsync(u => u.Id == receiverId.Value);
        if (!receiverExists)
        {
            return Result.Failure("Receiver does not exist.");
        }

        var directMessage = new DirectMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId.Value,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _db.DirectMessages.Add(directMessage);
        await _db.SaveChangesAsync();
        return Result.Success();
    }
}

