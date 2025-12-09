namespace SocialNetwork.Test.Services;

public class FollowsServiceTests
{
    private readonly Mock<IFollowRepository> _followRepositoryMock;
    private readonly IFollowsService _sut;
    public FollowsServiceTests()
    {
        _followRepositoryMock = new Mock<IFollowRepository>();
        _sut = new FollowService(_followRepositoryMock.Object);
    }

    [Fact]
    public async Task FollowAsync_WhenNotAlreadyFollowing_ReturnsSuccess()
    {
        // Arrange

        _followRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);
        // Act
        var result = await _sut.FollowAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task FollowAsync_WhenAlreadyFollowing_ReturnsFailure()
    {
        // Arrange
        _followRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.FollowAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.ErrorMessage, FollowErrors.AlreadyFollowing);
    }


    [Fact]
    public async Task FollowAsync_WhenTryingToFollowYourself_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.FollowAsync(userId, userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.ErrorMessage, FollowErrors.CannotFollowSelf);
    }

    [Fact]
    public async Task UnfollowAsync_FollowerRelationshipDontExist_ReturnsFailure()
    {
        // Arrange
        _followRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await _sut.UnfollowAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.ErrorMessage,FollowErrors.UnableToUnfollow);
    }

    [Fact]
    public async Task UnfollowAsync_FollowerRelationshipExists_ReturnsTrue()
    {
        // Arrange
        _followRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.UnfollowAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetFollows_ValidUser_ReturnsArrayOfId()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followedUsers = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        _followRepositoryMock
            .Setup(r => r.GetFollowsAsync(followerId))
            .ReturnsAsync(followedUsers);

        // Act
        var result = await _sut.GetFollowsAsync(followerId);


        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(followedUsers, result.Data);
    }
    [Fact]
    public async Task GetFollows_EmptyUser_ReturnsError()
    {
        // Arrange
        var followerId = Guid.Empty;

        // Act
        var result = await _sut.GetFollowsAsync(followerId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.ErrorMessage, FollowErrors.EmptyUser);
        _followRepositoryMock.Verify(r => r.GetFollowsAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetFollows_ValidUserThatFollowsNoUsers_ReturnsEmptyList()
    {
        // Arrange
        _followRepositoryMock
            .Setup(r => r.GetFollowsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Guid[] { });

        // Act
        var result = await _sut.GetFollowsAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);

    }

    [Fact]
    public async Task IsFollowing_ValidUserThatFollowsId_ReturnsTrue()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _followRepositoryMock
            .Setup(r => r.ExistsAsync(followerId, followeeId))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.IsFollowingAsync(followerId, followeeId);

        // Assert
        Assert.True(result.Data);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task IsFollowing_ValidUserDontFollowId_ReturnsFalse()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _followRepositoryMock
            .Setup(r => r.ExistsAsync(followerId, followeeId))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.IsFollowingAsync(followerId, followeeId);

        // Assert
        Assert.False(result.Data);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task IsFollowing_EmptyUser_ReturnsError()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.Empty;

        // Act
        var result = await _sut.IsFollowingAsync(followerId, followeeId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(FollowErrors.EmptyUser, result.ErrorMessage);
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followedUsers = new[]
        {
        new FollowedUserDto { Id = Guid.NewGuid(), Username = "alice" },
        new FollowedUserDto { Id = Guid.NewGuid(), Username = "bob" }
    };

        _followRepositoryMock
            .Setup(r => r.GetFollowsWithUserInfoAsync(followerId))
            .ReturnsAsync(followedUsers);

        // Act
        var result = await _sut.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Length);
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_ValidUser_ReturnsCorrectUsernames()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followedUsers = new[]
        {
        new FollowedUserDto { Id = Guid.NewGuid(), Username = "alice" },
        new FollowedUserDto { Id = Guid.NewGuid(), Username = "bob" }
    };

        _followRepositoryMock
            .Setup(r => r.GetFollowsWithUserInfoAsync(followerId))
            .ReturnsAsync(followedUsers);

        // Act
        var result = await _sut.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        Assert.Contains(result.Data, u => u.Username == "alice");
        Assert.Contains(result.Data, u => u.Username == "bob");
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_EmptyGuid_ReturnsFailure()
    {
        // Arrange
        var followerId = Guid.Empty;

        // Act
        var result = await _sut.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Empty", result.ErrorMessage);
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_EmptyGuid_DoesNotCallRepository()
    {
        // Arrange
        var followerId = Guid.Empty;

        // Act
        await _sut.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        _followRepositoryMock.Verify(
            r => r.GetFollowsWithUserInfoAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_UserFollowsNobody_ReturnsEmptyArraySuccess()
    {
        // Arrange
        var followerId = Guid.NewGuid();

        _followRepositoryMock
            .Setup(r => r.GetFollowsWithUserInfoAsync(followerId))
            .ReturnsAsync(Array.Empty<FollowedUserDto>());

        // Act
        var result = await _sut.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var followerId = Guid.NewGuid();

        _followRepositoryMock
            .Setup(r => r.GetFollowsWithUserInfoAsync(followerId))
            .ReturnsAsync(Array.Empty<FollowedUserDto>());

        // Act
        await _sut.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        _followRepositoryMock.Verify(
            r => r.GetFollowsWithUserInfoAsync(followerId),
            Times.Once);
    }

}

