using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Test.Controllers;

public class FollowControllerTests
{

    private readonly Mock<IFollowsService> _followsUserServiceMock;
    private readonly FollowController _sut;
    public FollowControllerTests()
    {
        _followsUserServiceMock = new Mock<IFollowsService>();
         _sut = new FollowController( _followsUserServiceMock.Object );
    }

    [Fact]
    public async Task FollowUser_ValidRequest_ReturnsOk()
    {
        //Arrange
        _followsUserServiceMock.Setup( m => 
        m.FollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Success);
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        var request = new FollowRequest { FollowerId = followerId, FolloweeId = followeeId };

        //Act
        var reply = await _sut.Follow(request);

        // Assert
        Assert.IsType<OkResult>(reply);
        _followsUserServiceMock.Verify(
            s =>s.FollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task FollowUser_InvalidRequest_ReturnsBadRequest()
    {
        {
            //Arrange
            _followsUserServiceMock.Setup(m =>
            m.FollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Failure("Error"));
            var followerId = Guid.NewGuid();
            var followeeId = Guid.NewGuid();

            var request = new FollowRequest { FollowerId = followerId, FolloweeId = followeeId };

            //Act
            var reply = await _sut.Follow(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(reply);
            Assert.Equal("Error", badRequestResult.Value);
            _followsUserServiceMock.Verify(
                s => s.FollowAsync(followerId, followeeId), Times.Once());
        }
    }


}
