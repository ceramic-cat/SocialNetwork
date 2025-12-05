using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly IPostRepository _postRepository;

        public TimelineService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<List<Post>> GetPostsByUserIdAsync(Guid userId)
        {
            var posts = await _postRepository.GetPostsByUserIdAsync(userId);

            return posts
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }
    }
}
