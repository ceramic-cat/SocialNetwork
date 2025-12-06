using SocialNetwork.Entity.Models;

public interface IPostRepository
{
    Task<List<Post>> GetPostsByUserIdAsync(Guid userId);
    Task AddPostAsync(Post post);
    Task<Post?> GetByIdAsync(Guid postId);
    Task DeletePostAsync(Post post);
}
