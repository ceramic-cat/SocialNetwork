using SocialNetwork.Repository.Interfaces;
using SocialNetwork.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Test.Controllers
{
  public class TheFeedControllerTests
  {
    private readonly Mock<IFollowsService> _followsServiceMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly TheFeedController _controller;
    private readonly SocialNetworkDbContext _dbContext;

    public TheFeedControllerTests()
    {
      _followsServiceMock = new Mock<IFollowsService>();
      _postRepositoryMock = new Mock<IPostRepository>();
      var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDb")
          .Options;
      _dbContext = new SocialNetworkDbContext(options);

      _controller = new TheFeedController(_followsServiceMock.Object, _postRepositoryMock.Object, _dbContext);
    }

    [Fact]
    public async Task GetFeed_ReturnsOrderedPosts_WhenUserFollowsOthers()
    {
      var userId = Guid.NewGuid();
      var followedUserIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
      _followsServiceMock.Setup(s => s.GetFollowsAsync(userId))
          .ReturnsAsync(Result<Guid[]>.Success(followedUserIds));

      var posts = new List<Post>
            {
                new Post { Id = Guid.NewGuid(), SenderId = followedUserIds[0], ReceiverId = Guid.NewGuid(), Content = "First", CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
                new Post { Id = Guid.NewGuid(), SenderId = followedUserIds[1], ReceiverId = Guid.NewGuid(), Content = "Second", CreatedAt = DateTime.UtcNow }
            };
      _postRepositoryMock.Setup(r => r.GetPostsByUserIdsAsync(followedUserIds))
          .ReturnsAsync(posts);

      var result = await _controller.GetFeed(userId);

      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var returnedPosts = Assert.IsAssignableFrom<IEnumerable<PostDto>>(okResult.Value);
      Assert.Collection(returnedPosts,
          post => Assert.Equal("Second", post.Content),
          post => Assert.Equal("First", post.Content));
    }

    [Fact]
    public async Task GetFeed_ReturnsEmptyList_WhenNoFollows()
    {
      var userId = Guid.NewGuid();
      _followsServiceMock.Setup(s => s.GetFollowsAsync(userId))
          .ReturnsAsync(Result<Guid[]>.Success(Array.Empty<Guid>()));

      _postRepositoryMock.Setup(r => r.GetPostsByUserIdsAsync(It.IsAny<Guid[]>()))
          .ReturnsAsync(new List<Post>());

      var result = await _controller.GetFeed(userId);

      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var returnedPosts = Assert.IsAssignableFrom<IEnumerable<PostDto>>(okResult.Value);
      Assert.Empty(returnedPosts);
    }

    [Fact]
    public async Task GetFeed_ReturnsBadRequest_WhenFollowServiceFails()
    {
      var userId = Guid.NewGuid();
      _followsServiceMock.Setup(s => s.GetFollowsAsync(userId))
          .ReturnsAsync(Result<Guid[]>.Failure("error"));

      var result = await _controller.GetFeed(userId);

      Assert.IsType<BadRequestObjectResult>(result.Result);
    }
  }
}