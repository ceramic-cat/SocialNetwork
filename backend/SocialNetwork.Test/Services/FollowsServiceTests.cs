using Moq;
using SocialNetwork.Entity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        Assert.Contains(result.ErrorMessage, "Already following this user");
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
        Assert.Contains(result.ErrorMessage, "You can't follow yourself");
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
        Assert.Contains(result.ErrorMessage, "Unable to unfollow that user");
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
        Assert.Contains(result.ErrorMessage, "Empty user");
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


}

