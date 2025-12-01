using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Services
{
    public class AuthService : IAuthService
    {
        private static readonly List<User> _users = new();

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (_users.Any(u => u.Username == request.Username))
                return false;

            var user = new User
            {
                Username = request.Username,
                Password = request.Password 
            };
            _users.Add(user);
            return true;
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            var user = _users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null)
                return null;

            // Return a dummy token for now
            return "dummy-jwt-token";
        }
    }
}