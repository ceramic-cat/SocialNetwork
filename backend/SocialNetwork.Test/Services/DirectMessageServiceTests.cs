using System;
using System.Threading.Tasks;
using Moq;
using SocialNetwork.Entity;
using SocialNetwork.Repository.Interfaces;
using SocialNetwork.Repository.Services;
using Xunit;

namespace SocialNetwork.Test.Services
{
    public class DirectMessageServiceTests
    {
        private readonly Mock<IDirectMessageRepository> _dmRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly DirectMessageService _sut;

        public DirectMessageServiceTests()
        {
            _dmRepositoryMock = new Mock<IDirectMessageRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _sut = new DirectMessageService(
                _dmRepositoryMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenReceiverDoesNotExist()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = "Hello";

            _userRepositoryMock
                .Setup(r => r.ExistsAsync(receiverId))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, message);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Receiver does not exist.", result.ErrorMessage);

            _userRepositoryMock.Verify(
                r => r.ExistsAsync(receiverId),
                Times.Once
            );

            _dmRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<DirectMessage>()),
                Times.Never
            );
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenMessageIsEmpty()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = string.Empty;

            _userRepositoryMock
                .Setup(r => r.ExistsAsync(receiverId))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, message);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Message cannot be empty.", result.ErrorMessage);

            _dmRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<DirectMessage>()),
                Times.Never
            );
        }

        [Fact]
        public async Task SendDirectMessageAsync_ReturnsFail_WhenMessageIsTooLong()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var longMessage = new string('x', 281);

            _userRepositoryMock
                .Setup(r => r.ExistsAsync(receiverId))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, longMessage);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Message cannot be longer than 280 characters.", result.ErrorMessage);

            _dmRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<DirectMessage>()),
                Times.Never
            );
        }

        [Fact]
        public async Task SendDirectMessageAsync_ShouldSaveMessage_WhenRequestIsValid()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = "This is a valid message.";

            _userRepositoryMock
                .Setup(r => r.ExistsAsync(receiverId))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.SendDirectMessageAsync(senderId, receiverId, message);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);

            _userRepositoryMock.Verify(
                r => r.ExistsAsync(receiverId),
                Times.Once
            );

            _dmRepositoryMock.Verify(
                r => r.AddAsync(It.Is<DirectMessage>(dm =>
                    dm.SenderId == senderId &&
                    dm.ReceiverId == receiverId &&
                    dm.Message == message
                )),
                Times.Once
            );
        }
    }
}

