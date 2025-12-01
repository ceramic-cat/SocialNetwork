using SocialNetwork.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Repository.Services
{
    public class AuthService : IAuthService
    {
        private readonly SocialNetworkDbContext _db;

        public AuthService(SocialNetworkDbContext db)
        {
            _db = db;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
                return false;

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                Email = request.Email,
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null)
                return null;

            // Return a dummy token for now
            return "dummy-jwt-token";
        }
    }
}