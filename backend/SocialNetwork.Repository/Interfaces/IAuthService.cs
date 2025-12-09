using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<string?> LoginAsync(LoginRequest request);
        Task<bool> DeleteAccountAsync(Guid userId);
        Task<bool> EditProfileAsync(Guid userId, EditProfileRequest request);
        Task<string?> GetUsernameAsync(Guid userId);
    }
}