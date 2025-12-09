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
}

