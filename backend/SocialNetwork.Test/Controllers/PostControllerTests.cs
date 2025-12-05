
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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
                .ReturnsAsync(PostResult.Fail("Content cannot be empty."));

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<PostResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal("Content cannot be empty.", body.ErrorMessage);

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

            var errorContent = "Sender does not exist.";

            _postServiceMock
                .Setup(s => s.CreatePostAsync(senderId, receiverId, content))
                .ReturnsAsync(PostResult.Fail(errorContent));

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

            var body = Assert.IsType<PostResult>(badRequest.Value);
            Assert.False(body.Success);
            Assert.Equal(errorContent, body.ErrorMessage);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, content),
                Times.Once
            );
        }
    }
}
