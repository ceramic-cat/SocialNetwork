using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Repositories;
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

    [Fact]
    public async Task GetFollowsAsync_ValidRequest_ReturnsOk()
    {
        // Arrange
        var followedUsers = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

        _followServiceMock
            .Setup(m => m.GetFollowsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Guid[]>.Success(followedUsers));

        // Act
        var result = await _sut.GetFollows(Guid.NewGuid());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGuids = Assert.IsType<Guid[]>(okResult.Value);
        Assert.Equal(3, returnedGuids.Length);
        Assert.Equal(followedUsers, returnedGuids);
    }
    [Fact]
    public async Task GetFollowsAsync_EmptyGuid_ReturnsBadRequest()
    {
        // Arrange
        _followServiceMock
            .Setup(m => m.GetFollowsAsync(Guid.Empty))
            .ReturnsAsync(Result<Guid[]>.Failure("Empty user"));

        // Act
        var result = await _sut.GetFollows(Guid.Empty);

        // Assert
        var badrequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Empty user", badrequestResult.Value);
    }
}




