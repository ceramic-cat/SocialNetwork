
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
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

        [Fact]
        public async Task DeletePostAsync_ReturnsFail_WhenPostDoesNotExist()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();


            //Act
            var result = await _sut.DeletePostAsync(postId, userId);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("Post not found.", result.ErrorMessage);
        }


        [Fact]
        public async Task DeletePostAsync_DeletesPost_WhenUserIsOwner()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var post = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ReceiverId = Guid.NewGuid(),
                Content = "Sample post",
                CreatedAt = DateTime.UtcNow
            };
                _db.Posts.Add(post);
                await _db.SaveChangesAsync();
            //Act

            var result = await _sut.DeletePostAsync(post.Id, userId);
            //Assert

            Assert.True(result.Success);
            var exists = await _db.Posts.AnyAsync(p => p.Id == post.Id);
            Assert.False(exists);

        }
        [Fact]
        public async Task DeletePostAsync_ReturnsFail_WhenUserIsNotOwner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ReceiverId = userId,
                Content = "Owned by someone else",
                CreatedAt = DateTime.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            // Act
            var result = await _sut.DeletePostAsync(post.Id, otherUserId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("You are not allowed to delete this post.", result.ErrorMessage);

            // Post ska fortfarande finnas
            var stillThere = await _db.Posts.AnyAsync(p => p.Id == post.Id);
            Assert.True(stillThere);
        }

    }
}
