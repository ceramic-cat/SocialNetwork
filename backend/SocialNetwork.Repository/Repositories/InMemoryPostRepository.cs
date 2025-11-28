using System.Collections.Concurrent;
using System.Threading.Tasks;
using SocialNetwork.Entity;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Repositories
{
    public class InMemoryPostRepository : IPostRepository
    {
        private readonly ConcurrentBag<Post> _posts = new();

        public Task AddAsync(Post post)
        {
            _posts.Add(post);
            return Task.CompletedTask;
        }
    }
}
