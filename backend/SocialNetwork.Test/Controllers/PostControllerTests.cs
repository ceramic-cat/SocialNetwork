
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
            var message = "Success post!";

            var request = new CreatePostRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };

            _postServiceMock
                .Setup(s => s.CreatePostAsync(senderId, receiverId, message))
                .ReturnsAsync(PostResult.Ok());

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, message),
                Times.Once
            );
        }

        [Fact]
        public async Task CreatePost_EmptyMessage_ReturnsBadRequest()
        {
            // Arrange
            var sender = Guid.NewGuid();
            var receiver = Guid.NewGuid();
            var message = "   "; 

            var request = new CreatePostRequest
            {
                SenderId = sender,
                ReceiverId = receiver,
                Message = message
            };

            _postServiceMock
                .Setup(s => s.CreatePostAsync(sender, receiver, message))
                .ReturnsAsync(PostResult.Fail("Message cannot be empty."));

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Message cannot be empty.", badRequest.Value);

            _postServiceMock.Verify(s =>
                s.CreatePostAsync(sender, receiver, message),
                Times.Once
            );
        }
        [Fact]
        public async Task CreatePost_SenderidDoesntExist_ReturnsBadRequestWithErrorMessage()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = "This will fail";

            var request = new CreatePostRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };

            var errorMessage = "Sender does not exist.";

            _postServiceMock
                .Setup(s => s.CreatePostAsync(senderId, receiverId, message))
                .ReturnsAsync(PostResult.Fail(errorMessage));

            // Act
            var result = await _sut.CreatePost(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Equal(errorMessage, badRequest.Value);

            _postServiceMock.Verify(
                s => s.CreatePostAsync(senderId, receiverId, message),
                Times.Once
            );
        }

    }
}
