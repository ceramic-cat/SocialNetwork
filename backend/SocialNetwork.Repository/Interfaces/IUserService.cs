using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Services
{
    public interface IUserService
    {
        Task<List<User>> SearchUsersAsync(string query);
    }
}
