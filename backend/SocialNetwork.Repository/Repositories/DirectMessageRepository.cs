using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Repositories;

public class DirectMessageRepository : IDirectMessageRepository
{
    private readonly SocialNetworkDbContext _db;

    public DirectMessageRepository(SocialNetworkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(DirectMessage directMessage)
    {
        _db.DirectMessages.Add(directMessage);
        await _db.SaveChangesAsync();
    }

    public async Task<List<DirectMessage>> GetByUserIdAsync(Guid userId)
    {
        return await _db.DirectMessages
            .Where(dm => dm.SenderId == userId || dm.ReceiverId == userId)
            .ToListAsync();
    }
}

