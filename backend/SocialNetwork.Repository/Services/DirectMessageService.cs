using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Errors;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Services;

    public class DirectMessageService : IDirectMessageService
    {
    private readonly SocialNetworkDbContext _db;
    private readonly IDirectMessageRepository _directMessageRepository;
    private const int MaxContentLength = 280;

    public DirectMessageService(SocialNetworkDbContext db, IDirectMessageRepository directMessageRepository)
        {
        _db = db;
        _directMessageRepository = directMessageRepository;
        }

    public async Task<Result> SendDirectMessageAsync(Guid senderId, Guid receiverId, string content)
        {
        var validationResult = ValidateInputs(senderId, receiverId, content);
        if (validationResult != null)
        {
            return validationResult;
        }

        var usersExistResult = await ValidateUsersExistAsync(senderId, receiverId);
        if (usersExistResult != null)
            {
            return usersExistResult;
        }

        await SaveDirectMessageAsync(senderId, receiverId, content);
        return Result.Success();
    }

    private Result? ValidateInputs(Guid senderId, Guid receiverId, string content)
    {
        if (senderId == Guid.Empty)
        {
            return Result.Failure(DirectMessageErrors.SenderEmpty);
            }

        if (receiverId == Guid.Empty)
        {
            return Result.Failure(DirectMessageErrors.ReceiverEmpty);
        }

        if (string.IsNullOrWhiteSpace(content))
            {
            return Result.Failure(DirectMessageErrors.ContentEmpty);
            }

        if (content.Length > MaxContentLength)
            {
            return Result.Failure(DirectMessageErrors.ContentTooLong);
        }

        return null;
    }

    private async Task<Result?> ValidateUsersExistAsync(Guid senderId, Guid receiverId)
    {
        var userIds = new[] { senderId, receiverId };
        var existingUserIds = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        if (!existingUserIds.Contains(senderId))
        {
            return Result.Failure(DirectMessageErrors.SenderDoesNotExist);
        }

        if (!existingUserIds.Contains(receiverId))
        {
            return Result.Failure(DirectMessageErrors.ReceiverDoesNotExist);
            }

        return null;
    }

    private async Task SaveDirectMessageAsync(Guid senderId, Guid receiverId, string content)
    {
            var directMessage = new DirectMessage
            {
            Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
            Content = content,
                CreatedAt = DateTime.UtcNow
            };

        await _directMessageRepository.AddAsync(directMessage);
    }

    public async Task<List<DirectMessage>> GetMessagesByUserIdAsync(Guid userId)
    {
        var messages = await _directMessageRepository.GetByUserIdAsync(userId);

        return messages
            .OrderByDescending(m => m.CreatedAt)
            .ToList();
    }
}

