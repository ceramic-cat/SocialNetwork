using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Repositories
{
    public class SqliteUserRepository : IUserRepository
    {
        private readonly SocialNetworkDbContext _dbContext;

        public SqliteUserRepository(SocialNetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == userId);
        }
    }
}

