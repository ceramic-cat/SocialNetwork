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
        var mockUserRepo = new Mock<IUserRepository>();

        mockRepo.Setup(r => r.IsFollowingAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);
        mockUserRepo.Setup(r => r.IsValidUser(It.IsAny<Guid>()))
            .ReturnsAsync(true);


        var service = new FollowService(mockRepo.Object);
        
        // Act
        var result = await service.FollowUserAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);
    }

}

