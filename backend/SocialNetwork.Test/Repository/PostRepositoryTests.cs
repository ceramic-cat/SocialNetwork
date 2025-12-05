using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;           
using SocialNetwork.Repository;
namespace SocialNetwork.Test.Repository
{
    public class PostRepositoryTests
    {
        private readonly SocialNetworkDbContext _db;
        private readonly PostRepository _sut;

        public PostRepositoryTests()
        {
                        var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _db = new SocialNetworkDbContext(options);
            _sut = new PostRepository(_db);

        }

        [Fact]
        public async Task GetPostsByUserIdAsync_ReturnsOnlyPostsMatchingRecieverId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var postOneForUser = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                ReceiverId = userId,
                Content = "Post for user",
                CreatedAt = DateTime.UtcNow
            };
            var postTwoForUser = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                ReceiverId = userId,
                Content = "Another post for user",
                CreatedAt = DateTime.UtcNow
            };
            var postForOtherUser = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                ReceiverId = otherUserId,
                Content = "Post for other user",
                CreatedAt = DateTime.UtcNow
            };
            _db.Posts.AddRange(postOneForUser, postTwoForUser, postForOtherUser);
            await _db.SaveChangesAsync();
            // Act
            var result = await _sut.GetPostsByUserIdAsync(userId);
            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, post => Assert.Equal(userId, post.ReceiverId));
        }
    }
}
