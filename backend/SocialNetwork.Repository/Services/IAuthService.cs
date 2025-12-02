using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<string?> LoginAsync(LoginRequest request);
    }
}