namespace SocialNetwork.Test.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<IPostService> _postServiceMock;
        private readonly PostsController _sut;

        public PostsControllerTests()
        {
            _postServiceMock = new Mock<IPostService>();
            _sut = new PostsController(_postServiceMock.Object);
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
        public async Task CreatePost_ValidRequest_ReturnsOk()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Success post!";

            SetUserWithId(senderId);

            var request = new CreatePostRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _postServiceMock
                .Setup(s => s.CreatePostAsync(senderId, receiverId, content))
                .ReturnsAsync(PostResult.Ok());

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<PostResult>(okResult.Value);
            Assert.True(body.Success);
            Assert.Null(body.ErrorMessage);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task CreatePost_EmptyContent_ReturnsBadRequest()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "   ";

            SetUserWithId(senderId);

            var request = new CreatePostRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _postServiceMock
                .Setup(s => s.CreatePostAsync(senderId, receiverId, content))
                .ReturnsAsync(PostResult.Fail(PostErrors.ContentEmpty));

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<PostResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(PostErrors.ContentEmpty, body.ErrorMessage);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task CreatePost_SenderDoesntExist_ReturnsBadRequestWithErrorContent()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "This will fail";

            SetUserWithId(senderId);

            var request = new CreatePostRequest
            {
                ReceiverId = receiverId,
                Content = content
            };

            _postServiceMock
                .Setup(s => s.CreatePostAsync(senderId, receiverId, content))
                .ReturnsAsync(PostResult.Fail(PostErrors.SenderDoesNotExist));

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<PostResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(PostErrors.SenderDoesNotExist, body.ErrorMessage);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task DeletePost_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            SetUserWithId(userId);

            _postServiceMock
                .Setup(s => s.DeletePostAsync(postId, userId))
                .ReturnsAsync(PostResult.Ok());

            // Act
            var result = await _sut.DeletePost(postId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<PostResult>(ok.Value);
            Assert.True(body.Success);
            Assert.Null(body.ErrorMessage);

            _postServiceMock.Verify(
                s => s.DeletePostAsync(postId, userId),
                Times.Once);
        }

        [Fact]
        public async Task DeletePost_InvalidUserIdInToken_ReturnsUnauthorized()
        {
            // Arrange
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var postId = Guid.NewGuid();

            // Act
            var result = await _sut.DeletePost(postId);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var body = Assert.IsType<PostResult>(unauthorized.Value);
            Assert.False(body.Success);
            Assert.Equal(PostErrors.InvalidUserId, body.ErrorMessage);

            _postServiceMock.Verify(
                s => s.DeletePostAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public async Task DeletePost_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            SetUserWithId(userId);

            _postServiceMock
                .Setup(s => s.DeletePostAsync(postId, userId))
                .ReturnsAsync(PostResult.Fail(PostErrors.PostNotFound));

            // Act
            var result = await _sut.DeletePost(postId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var body = Assert.IsType<PostResult>(notFound.Value);
            Assert.False(body.Success);
            Assert.Equal(PostErrors.PostNotFound, body.ErrorMessage);

            _postServiceMock.Verify(
                s => s.DeletePostAsync(postId, userId),
                Times.Once);
        }
        [Fact]
        public async Task DeletePost_UserNotOwner_ReturnsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            SetUserWithId(userId);

            _postServiceMock
                .Setup(s => s.DeletePostAsync(postId, userId))
                .ReturnsAsync(PostResult.Fail(PostErrors.NotAllowedToDelete));

            // Act
            var result = await _sut.DeletePost(postId);

            // Assert
            var forbidden = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, forbidden.StatusCode);

            var body = Assert.IsType<PostResult>(forbidden.Value);
            Assert.False(body.Success);
            Assert.Equal(PostErrors.NotAllowedToDelete, body.ErrorMessage);
        }
    }
}
