using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.Test.Services
{
    public class DirectMessageServiceTests
    {
        private readonly SocialNetworkDbContext _db;
        private readonly DirectMessageService _sut;

        public DirectMessageServiceTests()
        {
            var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .Options;

            _db = new SocialNetworkDbContext(options);
            _sut = new DirectMessageService(_db);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenSenderIdIsEmpty()
        {
            // Arrange
            var senderId = Guid.Empty;
            var receiverId = Guid.NewGuid();
            var content = "Hello, World!";

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("SenderId cannot be empty.", result.ErrorMessage);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenContentIsEmpty()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = string.Empty;

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Content cannot be empty.", result.ErrorMessage);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenContentIsTooLong()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var longContent = new string('x', 281);

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, longContent);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Content cannot be longer than 280 characters.", result.ErrorMessage);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenSenderDoesNotExist()
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
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Sender does not exist.", result.ErrorMessage);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenReceiverIdIsEmpty()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.Empty;
            var content = "Hello, World!";

            _db.Users.Add(new User
            {
                Id = senderId,
                Username = "sender",
                Email = "sender@example.com",
                Password = "pw1234as!",
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            });
            await _db.SaveChangesAsync();

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ReceiverId cannot be empty.", result.ErrorMessage);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenReceiverDoesNotExist()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Hello, World!";

            _db.Users.Add(new User
            {
                Id = senderId,
                Username = "sender",
                Email = "sender@example.com",
                Password = "pw1234as!",
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            });
            await _db.SaveChangesAsync();

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Receiver does not exist.", result.ErrorMessage);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenReceiverIdIsNull()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            Guid? receiverId = null;
            var content = "Hello, World!";

            _db.Users.Add(new User
            {
                Id = senderId,
                Username = "sender",
                Email = "sender@example.com",
                Password = "pw1234as!",
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            });
            await _db.SaveChangesAsync();

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ReceiverId cannot be empty.", result.ErrorMessage);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
        }

        [Fact]
        public async Task SendDirectMessageAsync_ShouldSaveMessage_WhenAllInputsAreValid()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "This is a valid message.";

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
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorMessage);

            var message = await _db.DirectMessages.SingleAsync();
            Assert.Equal(senderId, message.SenderId);
            Assert.Equal(receiverId, message.ReceiverId);
            Assert.Equal(content, message.Content);
            Assert.NotEqual(default, message.CreatedAt);

            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(1, messagesInDb);
        }
    }
}

