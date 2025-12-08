namespace SocialNetwork.Test.Controllers
{
    public class DmControllerTests
    {
        private readonly Mock<IDirectMessageService> _directMessageServiceMock;
        private readonly DmController _sut;

        public DmControllerTests()
        {
            _directMessageServiceMock = new Mock<IDirectMessageService>();
            _sut = new DmController(_directMessageServiceMock.Object);
        }

        private void SetUserWithId(Guid userId)
        {
            var claims = new[]
            {
                new Claim("UserId", userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };
        }

        [Fact]
        public async Task SendDirectMessage_ValidRequest_ReturnsOk()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Hello, this is a valid message!";

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(DirectMessageResult.Ok());

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(okResult.Value);
            Assert.True(body.Success);
            Assert.Null(body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_NoToken_ReturnsUnauthorized()
        {
            // Arrange
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = Guid.NewGuid(),
                Content = "Message without token"
            };

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(unauthorized.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.InvalidUserId, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task SendDirectMessage_InvalidGuidInToken_ReturnsUnauthorized()
        {
            // Arrange
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("UserId", "not-a-valid-guid")
                    }, "TestAuth"))
                }
            };

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = Guid.NewGuid(),
                Content = "Message with invalid token"
            };

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(unauthorized.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.InvalidUserId, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task SendDirectMessage_NullUserIdClaim_ReturnsUnauthorized()
        {
            // Arrange
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("UserId", (string?)null!)
                    }, "TestAuth"))
                }
            };

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = Guid.NewGuid(),
                Content = "Message with null claim"
            };

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(unauthorized.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.InvalidUserId, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task SendDirectMessage_ContentEmpty_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "   ";

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(DirectMessageResult.Fail(DirectMessageErrors.ContentEmpty));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.ContentEmpty, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_ContentTooLong_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var longContent = new string('x', 281);

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = longContent
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, longContent))
                .ReturnsAsync(DirectMessageResult.Fail(DirectMessageErrors.ContentTooLong));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.ContentTooLong, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, longContent),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_ReceiverEmpty_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.Empty;
            var content = "This will fail due to empty receiver";

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(DirectMessageResult.Fail(DirectMessageErrors.ReceiverEmpty));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.ReceiverEmpty, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_SenderDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "This will fail";

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(DirectMessageResult.Fail(DirectMessageErrors.SenderDoesNotExist));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.SenderDoesNotExist, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_ReceiverDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "This will fail";

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(DirectMessageResult.Fail(DirectMessageErrors.ReceiverDoesNotExist));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(DirectMessageErrors.ReceiverDoesNotExist, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_ContentExactly280Characters_ReturnsOk()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = new string('x', 280);

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(DirectMessageResult.Ok());

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<DirectMessageResult>(okResult.Value);
            Assert.True(body.Success);
            Assert.Null(body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

    }
}
