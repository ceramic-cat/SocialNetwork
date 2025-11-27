
namespace SocialNetwork.Test.Services
{
    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly PostService _sut; 

        public PostServiceTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();

            _sut = new PostService(
                _postRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreatePostAsync_ReturnsFail_WhenMessageIsEmpty()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = string.Empty;

            // Act
            var result = await _sut.CreatePostAsync(senderId, receiverId, message);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Message cannot be empty.", result.ErrorMessage);

            _postRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Post>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreatePostAsync_ReturnsFail_WhenMessageIsTooLong()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var longMessage = new string('x', 281); 

            // Act
            var result = await _sut.CreatePostAsync(senderId, receiverId, longMessage);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Message cannot be longer than 280 characters.", result.ErrorMessage);

            _postRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Post>()),
                Times.Never
            );

        } 
        
        [Fact]

        public async Task CreatePostAsync_ShouldSavePost_WhenMessageIsValid()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var message = "This is a valid post message.";
            // Act
            var result = await _sut.CreatePostAsync(senderId, receiverId, message);
            // Assert
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            _postRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Post>(p =>
                    p.SenderId == senderId &&
                    p.ReceiverId == receiverId &&
                    p.Message == message
                )),
                Times.Once
            );
        }

    }
}
