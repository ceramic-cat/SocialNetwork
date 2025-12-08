using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.Test.Services
{
    public class DirectMessageServiceTests
    {
        private readonly SocialNetworkDbContext _db;
        private readonly DirectMessageService _sut;
        private const int MaxContentLength = 280;

        public DirectMessageServiceTests()
        {
            var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .Options;

            _db = new SocialNetworkDbContext(options);
            _sut = new DirectMessageService(_db);
        }

        private async Task<User> CreateUserAsync(Guid id, string username, string email)
        {
            var user = new User
            {
                Id = id,
                Username = username,
                Email = email,
                Password = "pw1234as!",
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        private async Task AssertNoMessagesInDatabaseAsync()
        {
            var messagesInDb = await _db.DirectMessages.CountAsync();
            Assert.Equal(0, messagesInDb);
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
            await AssertNoMessagesInDatabaseAsync();
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
            await AssertNoMessagesInDatabaseAsync();
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenContentIsTooLong()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var longContent = new string('x', MaxContentLength + 1);

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, longContent);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Content cannot be longer than {MaxContentLength} characters.", result.ErrorMessage);
            await AssertNoMessagesInDatabaseAsync();
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenSenderDoesNotExist()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Hello, World!";

            await CreateUserAsync(receiverId, "receiver", "receiver@example.com");

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Sender does not exist.", result.ErrorMessage);
            await AssertNoMessagesInDatabaseAsync();
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenReceiverIdIsEmpty()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.Empty;
            var content = "Hello, World!";

            await CreateUserAsync(senderId, "sender", "sender@example.com");

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ReceiverId cannot be empty.", result.ErrorMessage);
            await AssertNoMessagesInDatabaseAsync();
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenReceiverDoesNotExist()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Hello, World!";

            await CreateUserAsync(senderId, "sender", "sender@example.com");

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, content);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Receiver does not exist.", result.ErrorMessage);
            await AssertNoMessagesInDatabaseAsync();
        }

        [Fact]
        public async Task SendDirectMessageAsync_ShouldSaveMessage_WhenAllInputsAreValid()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "This is a valid message.";

            await CreateUserAsync(senderId, "sender", "sender@example.com");
            await CreateUserAsync(receiverId, "receiver", "receiver@example.com");

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

