using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Test.Controllers;

public class FollowControllerTests
{

    private readonly Mock<IFollowsService> _followServiceMock;
    private readonly FollowController _sut;
    public FollowControllerTests()
    {
        _followServiceMock = new Mock<IFollowsService>();
        _sut = new FollowController(_followServiceMock.Object);
    }

    [Fact]
    public async Task Follow_ValidRequest_ReturnsOk()
    {
        //Arrange
        _followServiceMock.Setup(m =>
        m.FollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Success);
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        var request = new FollowRequest { FollowerId = followerId, FolloweeId = followeeId };

        //Act
        var reply = await _sut.Follow(request);

        // Assert
        Assert.IsType<OkResult>(reply);
        _followServiceMock.Verify(
            s => s.FollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task Follow_InvalidRequest_ReturnsBadRequest()
    {

        //Arrange
        _followServiceMock.Setup(m =>
            m.FollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Failure("Error"));
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        var request = new FollowRequest { FollowerId = followerId, FolloweeId = followeeId };

        //Act
        var reply = await _sut.Follow(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(reply);
        Assert.Equal("Error", badRequestResult.Value);
        _followServiceMock.Verify(
            s => s.FollowAsync(followerId, followeeId), Times.Once());

    }

    [Fact]
    public async Task Unfollow_ValidRequest_ReturnsOk()
    {
        _followServiceMock.Setup(m =>
            m.UnfollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Success);
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        var request = new FollowRequest { FollowerId = followerId, FolloweeId = followeeId };

        //Act
        var response = await _sut.Unfollow(request);

        // Assert
        Assert.IsType<OkResult>(response);
        _followServiceMock.Verify(
            s => s.UnfollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task Unfollow_InvalidRequest_ReturnsBadRequest()
    {
        _followServiceMock.Setup(m =>
            m.UnfollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Failure("Error"));
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        var request = new FollowRequest { FollowerId = followerId, FolloweeId = followeeId };

        //Act
        var response = await _sut.Unfollow(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Error", badRequestResult.Value);
        _followServiceMock.Verify(
            s => s.UnfollowAsync(followerId, followeeId), Times.Once());
    }
}




