using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SocialNetwork.Entity;
namespace SocialNetwork.Test.Services;

public class FollowServiceTests
{

    [Fact]
    public async Task FollowUserAsync_WhenNotAlreadyFollowing_ReturnsSuccess()
    {
        // Arrange
        var mockRepo = new Mock<IFollowRepository>();

        mockRepo.Setup(r => r.IsFollowingAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);


        var service = new FollowService(mockRepo.Object);
        
        // Act
        var result = await service.FollowUserAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task FollowUserAsync_WhenAlreadyFollowing_ReturnsFailure()
    {
        // Arrange
        var mockRepo = new Mock<IFollowRepository>();

        mockRepo.Setup(r => r.IsFollowingAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var service = new FollowService(mockRepo.Object);

        // Act
        var result = await service.FollowUserAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.ErrorMessage, "Already following this user");
    }


    [Fact]
    public async Task FollowUserAsync_WhenTryingToFollowYourself_ReturnsFailure()
    {
        // Arrange
        var mockRepo = new Mock<IFollowRepository>();
        var userId = Guid.NewGuid();


        var service = new FollowService(mockRepo.Object);

        // Act
        var result = await service.FollowUserAsync(userId, userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.ErrorMessage, "You can't follow yourself");
    }

    [Fact]
    public async Task RemoveFollowerAsync_FollowerRelationshipDontExist_ReturnsFailure()
    {
        // Arrange
        var mockRepo = new Mock<IFollowRepository>();
        mockRepo.Setup(r=> r.IsFollowingAsync(Guid.NewGuid(), It.IsAny<Guid>())).ReturnsAsync(false);

        var service = new FollowService(mockRepo.Object);

        // Act
        var result = await service.UnfollowUserAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.ErrorMessage, "Unable to unfollow that user");
    }
}

