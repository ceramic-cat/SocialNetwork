
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;

namespace SocialNetwork.Test.Services
{
    public class PostServiceTests
    {
        private readonly SocialNetworkDbContext _db;
        private readonly PostService _sut; 

        public PostServiceTests()
        {
            var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .Options;

            _db = new SocialNetworkDbContext(options);
            _sut = new PostService(_db);
        }

        [Fact]
        public async Task CreatePostAsync_ReturnsFail_WhenContentIsEmpty()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = string.Empty;

            // Act
            var result = await _sut.CreatePostAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Content cannot be empty.", result.ErrorMessage);
           
            var postsInDb = await _db.Posts.CountAsync();
            Assert.Equal(0, postsInDb);
            
        }

        [Fact]
        public async Task CreatePostAsync_ReturnsFail_WhenContentIsTooLong()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var longContent = new string('x', 281); 

            // Act
            var result = await _sut.CreatePostAsync(senderId, receiverId, longContent);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Content cannot be longer than 280 characters.", result.ErrorMessage);

            var postsInDb = await _db.Posts.CountAsync();
            Assert.Equal(0, postsInDb);

        }

        [Fact]
        public async Task CreatePostAsync_ReturnsFail_WhenSenderDoesNotExist()
        {
            // Arrange
            var senderId = Guid.NewGuid(); 
            var receiverId = Guid.NewGuid();
            var content = "Hello, World!";
           
            _db.Users.Add(new User
            {
                Id = receiverId,
                Username = "receiver",
                Email = "receiver@example.com",
                Password = "pw1234as!",
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            });
            await _db.SaveChangesAsync();

            // Act
            var result = await _sut.CreatePostAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Sender does not exist.", result.ErrorMessage);

            var postsInDb = await _db.Posts.CountAsync();
            Assert.Equal(0, postsInDb);
        }

        [Fact]
        public async Task CreatePostAsync_ShouldSavePost_WhenContentIsValid()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "This is a valid post.";

            _db.Users.AddRange(
                new User
                {
                    Id = senderId,
                    Username = "sender",
                    Email = "sender@example.com",
                    Password = "pw1234as!",
                    Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
                },
                new User
                {
                    Id = receiverId,
                    Username = "receiver",
                    Email = "receiver@example.com",
                    Password = "pw1234as!",
                    Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
                }
            );

            await _db.SaveChangesAsync();

            // Act
            var result = await _sut.CreatePostAsync(senderId, receiverId, content);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);

            var post = await _db.Posts.SingleAsync();
            Assert.Equal(senderId, post.SenderId);
            Assert.Equal(receiverId, post.ReceiverId);
            Assert.Equal(content, post.Content);
            Assert.NotEqual(default, post.CreatedAt);

            var postsInDb = await _db.Posts.CountAsync();
            Assert.Equal(1, postsInDb);
        }


    }
}
