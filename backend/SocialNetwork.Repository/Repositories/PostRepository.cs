using SocialNetwork.Repository.Interfaces;
using SocialNetwork.Entity.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly SocialNetworkDbContext _db;

        public PostRepository(SocialNetworkDbContext db)
        {
            _db = db;
        }

        public async Task<List<Post>> GetPostsByUserIdAsync(Guid userId)
        {
            return await _db.Posts
                .Where(p => p.ReceiverId == userId)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByUserIdsAsync(Guid[] userIds)
        {
            return await _db.Posts
                .Where(p => userIds.Contains(p.SenderId))
                .ToListAsync();
        }
    }
}
