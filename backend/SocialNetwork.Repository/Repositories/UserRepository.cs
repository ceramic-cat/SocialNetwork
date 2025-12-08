using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SocialNetworkDbContext _db;

        public UserRepository(SocialNetworkDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> SearchByUsernameAsync(string query)
        {
            var normalized = query.Trim().ToLower();

            return await _db.Users
                .Where(u => u.Username.ToLower().Contains(normalized))
                .ToListAsync();
        }

    }
}
