
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

        [Fact]
        public async Task CreatePost_ValidRequest_ReturnsOk()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Success post!";

            var request = new CreatePostRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content
            };

            _postServiceMock
                .Setup(s => s.CreatePostAsync(senderId, receiverId, content))
                .ReturnsAsync(PostResult.Ok());

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, content),
                Times.Once
            );
        }

        [Fact]
        public async Task CreatePost_EmptyContent_ReturnsBadRequest()
        {
            // Arrange
            var sender = Guid.NewGuid();
            var receiver = Guid.NewGuid();
            var content = "   "; 

            var request = new CreatePostRequest
            {
                SenderId = sender,
                ReceiverId = receiver,
                Content = content
            };

            _postServiceMock
                .Setup(s => s.CreatePostAsync(sender, receiver, content))
                .ReturnsAsync(PostResult.Fail("Content cannot be empty."));

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Content cannot be empty.", badRequest.Value);

            _postServiceMock.Verify(s =>
                s.CreatePostAsync(sender, receiver, content),
                Times.Once
            );
        }
        [Fact]
        public async Task CreatePost_SenderidDoesntExist_ReturnsBadRequestWithErrorContent()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "This will fail";

            var request = new CreatePostRequest
            {
                SenderId = senderId,
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
            Assert.Equal(errorContent, badRequest.Value);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, content),
                Times.Once
            );
        }

    }
}
