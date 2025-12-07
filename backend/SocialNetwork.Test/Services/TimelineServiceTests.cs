namespace SocialNetwork.Test.Services
{
    public class TimelineServiceTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly TimelineService _sut;

        public TimelineServiceTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _sut = new TimelineService(_postRepositoryMock.Object);
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_ReturnsOnlyUsersPosts_SortedNewestFirst()
        {
            var userId = Guid.NewGuid();
            var olderPost = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                ReceiverId = userId,
                Content = "Older post",
                CreatedAt = new DateTime(2023, 1, 1)
            };
            var middlePost = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                ReceiverId = userId,
                Content = "Middle post",
                CreatedAt = new DateTime(2023, 6, 1)
            };

            var newerPost = new Post
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                ReceiverId = userId,
                Content = "Newer post",
                CreatedAt = new DateTime(2023, 12, 1)
            };

            _postRepositoryMock.Setup(r => r.GetPostsByUserIdAsync(userId))
                .ReturnsAsync(new List<Post> { middlePost, olderPost, newerPost });

            //act
            var result = await _sut.GetPostsByUserIdAsync(userId);

            //assert
            Assert.All(result, post => Assert.Equal(userId, post.ReceiverId));
            Assert.Equal(new[] { newerPost.Id, middlePost.Id, olderPost.Id }, result.Select(p => p.Id));
        }
    }
}
