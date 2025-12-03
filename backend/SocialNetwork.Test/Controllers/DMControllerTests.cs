using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialNetwork.API.Controllers;
using SocialNetwork.API.Models;
using SocialNetwork.Repository.Services;
using Xunit;

namespace SocialNetwork.Test.Controllers
{
    public class DMControllerTests
    {
        private readonly Mock<IDirectMessageService> _dmServiceMock;
        private readonly DMController _sut;

        public DMControllerTests()
        {
            _dmServiceMock = new Mock<IDirectMessageService>();
            _sut = new DMController(_dmServiceMock.Object);
        }

        [Fact]
        public async Task SendDM_ValidRequest_ReturnsOk()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = "Hello!";

            var request = new CreateDMRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };

            _dmServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, message))
                .ReturnsAsync(DirectMessageResult.Ok());

            // Act
            var result = await _sut.SendDM(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            _dmServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, message),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDM_ReceiverDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = "Hello";

            var request = new CreateDMRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };

            _dmServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, message))
                .ReturnsAsync(DirectMessageResult.Fail("Receiver does not exist."));

            // Act
            var result = await _sut.SendDM(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Equal("Receiver does not exist.", badRequest.Value);

            _dmServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, message),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDM_EmptyMessage_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = "   ";

            var request = new CreateDMRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };

            _dmServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, message))
                .ReturnsAsync(DirectMessageResult.Fail("Message cannot be empty."));

            // Act
            var result = await _sut.SendDM(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Message cannot be empty.", badRequest.Value);

            _dmServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, message),
                Times.Once
            );
        }
    }
}

