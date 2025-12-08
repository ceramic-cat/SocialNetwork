namespace SocialNetwork.Test.Controllers
{
    public class DirectMessageControllerTests
    {
        private readonly Mock<IDirectMessageService> _directMessageServiceMock;
        private readonly DirectMessageController _sut;

        public DirectMessageControllerTests()
        {
            _directMessageServiceMock = new Mock<IDirectMessageService>();
            _sut = new DirectMessageController(_directMessageServiceMock.Object);
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
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<Result>(okResult.Value);
            Assert.True(body.IsSuccess);
            Assert.Null(body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_InvalidUserIdInToken_ReturnsUnauthorized()
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

            var body = Assert.IsType<Result>(unauthorized.Value);
            Assert.False(body.IsSuccess);
            Assert.Equal(DirectMessageErrors.InvalidUserId, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never
            );
        }


        [Fact]
        public async Task SendDirectMessage_EmptyContent_ReturnsBadRequest()
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
                .ReturnsAsync(Result.Failure(DirectMessageErrors.ContentEmpty));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<Result>(badRequest.Value);
            Assert.False(body.IsSuccess);
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
            var content = new string('x', 281);

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(Result.Failure(DirectMessageErrors.ContentTooLong));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<Result>(badRequest.Value);
            Assert.False(body.IsSuccess);
            Assert.Equal(DirectMessageErrors.ContentTooLong, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_ReceiverEmpty_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.Empty;
            var content = "Valid content";

            SetUserWithId(senderId);

            var request = new CreateDirectMessageRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _directMessageServiceMock
                .Setup(s => s.SendDirectMessageAsync(senderId, receiverId, content))
                .ReturnsAsync(Result.Failure(DirectMessageErrors.ReceiverEmpty));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<Result>(badRequest.Value);
            Assert.False(body.IsSuccess);
            Assert.Equal(DirectMessageErrors.ReceiverEmpty, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_SenderDoesntExist_ReturnsBadRequest()
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
                .ReturnsAsync(Result.Failure(DirectMessageErrors.SenderDoesNotExist));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<Result>(badRequest.Value);
            Assert.False(body.IsSuccess);
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
                .ReturnsAsync(Result.Failure(DirectMessageErrors.ReceiverDoesNotExist));

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<Result>(badRequest.Value);
            Assert.False(body.IsSuccess);
            Assert.Equal(DirectMessageErrors.ReceiverDoesNotExist, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task SendDirectMessage_ContentAtMaxLength_ReturnsOk()
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
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _sut.SendDirectMessage(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<Result>(okResult.Value);
            Assert.True(body.IsSuccess);
            Assert.Null(body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.SendDirectMessageAsync(senderId, receiverId, content),
                Times.Once
            );
        }
    }
}
