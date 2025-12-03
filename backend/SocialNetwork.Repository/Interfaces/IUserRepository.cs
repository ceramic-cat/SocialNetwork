namespace SocialNetwork.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(Guid userId);
    }
}

