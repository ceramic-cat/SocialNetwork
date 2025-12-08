using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> SearchByUsernameAsync(string query);
    }
}
