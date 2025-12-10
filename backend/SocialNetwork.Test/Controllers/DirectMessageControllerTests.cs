namespace SocialNetwork.Test.Controllers
{
    public class DirectMessageControllerTests
    {
        private readonly Mock<IDirectMessageService> _directMessageServiceMock;
        private readonly SocialNetworkDbContext _db;
        private readonly DirectMessageController _sut;

        public DirectMessageControllerTests()
        {
            _directMessageServiceMock = new Mock<IDirectMessageService>();

            var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new SocialNetworkDbContext(options);
            _sut = new DirectMessageController(_directMessageServiceMock.Object, _db);
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

        [Fact]
        public async Task GetDirectMessages_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            SetUserWithId(userId);

            var message1Id = Guid.NewGuid();
            var message2Id = Guid.NewGuid();

            var messages = new List<DirectMessage>
            {
                new DirectMessage
                {
                    Id = message1Id,
                    SenderId = userId,
                    ReceiverId = otherUserId,
                    Content = "Message 1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                },
                new DirectMessage
                {
                    Id = message2Id,
                    SenderId = otherUserId,
                    ReceiverId = userId,
                    Content = "Message 2",
                    CreatedAt = DateTime.UtcNow
                }
            };

            var user1 = new User
            {
                Id = userId,
                Username = "user1",
                Email = "user1@example.com",
                Password = "password",
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            var user2 = new User
            {
                Id = otherUserId,
                Username = "user2",
                Email = "user2@example.com",
                Password = "password",
                Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            _db.Users.AddRange(user1, user2);
            await _db.SaveChangesAsync();

            _directMessageServiceMock
                .Setup(s => s.GetMessagesByUserIdAsync(userId))
                .ReturnsAsync(messages);

            // Act
            var result = await _sut.GetDirectMessages();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var dtos = Assert.IsType<List<DirectMessageDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);

            // Find DTOs by message ID to avoid ordering issues
            var dto1 = dtos.FirstOrDefault(d => d.Id == message1Id);
            var dto2 = dtos.FirstOrDefault(d => d.Id == message2Id);

            Assert.NotNull(dto1);
            Assert.Equal("user1", dto1.SenderUsername);
            Assert.Equal("user2", dto1.ReceiverUsername);
            Assert.Equal("Message 1", dto1.Content);

            Assert.NotNull(dto2);
            Assert.Equal("user2", dto2.SenderUsername);
            Assert.Equal("user1", dto2.ReceiverUsername);
            Assert.Equal("Message 2", dto2.Content);

            _directMessageServiceMock.Verify(
                s => s.GetMessagesByUserIdAsync(userId),
                Times.Once
            );
        }

        [Fact]
        public async Task GetDirectMessages_InvalidUserIdInToken_ReturnsUnauthorized()
        {
            // Arrange
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _sut.GetDirectMessages();

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);

            var body = Assert.IsType<Result>(unauthorized.Value);
            Assert.False(body.IsSuccess);
            Assert.Equal(DirectMessageErrors.InvalidUserId, body.ErrorMessage);

            _directMessageServiceMock.Verify(
                s => s.GetMessagesByUserIdAsync(It.IsAny<Guid>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetDirectMessages_ReturnsEmptyList_WhenUserHasNoMessages()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetUserWithId(userId);

            _directMessageServiceMock
                .Setup(s => s.GetMessagesByUserIdAsync(userId))
                .ReturnsAsync(new List<DirectMessage>());

            // Act
            var result = await _sut.GetDirectMessages();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var dtos = Assert.IsType<List<DirectMessageDto>>(okResult.Value);
            Assert.Empty(dtos);

            _directMessageServiceMock.Verify(
                s => s.GetMessagesByUserIdAsync(userId),
                Times.Once
            );
        }
    }
}
