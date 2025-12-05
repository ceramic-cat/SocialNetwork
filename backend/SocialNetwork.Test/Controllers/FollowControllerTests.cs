using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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
    public async Task Follow_ValidTokenAndGuid_ReturnsOk()
    {
        //Arrange
        var followeeId = Guid.NewGuid();
        var followerId = Guid.NewGuid();

        _followServiceMock.Setup(m =>
        m.FollowAsync(followerId, followeeId)).ReturnsAsync(Result.Success);

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim("UserId", followerId.ToString())
                }, "TestAuth"))
            }
        };

        //Act
        var reply = await _sut.Follow(followeeId);

        // Assert
        Assert.IsType<OkResult>(reply);
        _followServiceMock.Verify(
            s => s.FollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task Follow_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim("UserId", followerId.ToString())
                }, "TestAuth"))
            }
        };

        _followServiceMock
            .Setup(m => m.FollowAsync(followerId, followeeId))
            .ReturnsAsync(Result.Failure("Error"));

        // Act
        var result = await _sut.Follow(followeeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error", badRequestResult.Value);
        _followServiceMock.Verify(
            s => s.FollowAsync(followerId, followeeId), Times.Once());
    }


    [Fact]
    public async Task Follow_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _sut.Follow(Guid.NewGuid());

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }


    [Fact]
    public async Task Follows_WithInvalidGuidInToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserId", "not-a-valid-guid")
                }, "TestAuth"))
            }
        };

        // Act
        var result = await _sut.Follow(Guid.NewGuid());

        // Assert 
        Assert.IsType<UnauthorizedResult>(result);

    }

    [Fact]
    public async Task Unfollow_ValidRequest_ReturnsOk()
    {
        _followServiceMock.Setup(m =>
            m.UnfollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Success);
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                    new Claim("UserId", followerId.ToString())
                    }, "TestAuth"))
            }
        };

        //Act
        var response = await _sut.Unfollow(followeeId);

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

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
{
                new Claim("UserId", followerId.ToString())
                }, "TestAuth"))
            }
        };

        //Act
        var response = await _sut.Unfollow(followeeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Error", badRequestResult.Value);
        _followServiceMock.Verify(
            s => s.UnfollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task Unfollow_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _sut.Unfollow(Guid.NewGuid());

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Unfollow_WithInvalidGuidInToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserId", "not-a-valid-guid")
                }, "TestAuth"))
            }
        };

        // Act
        var result = await _sut.Unfollow(Guid.NewGuid());

        // Assert 
        Assert.IsType<UnauthorizedResult>(result);

    }


    [Fact]
    public async Task GetFollowsAsync_WithValidToken_ReturnsOkWithGuids()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var followedUsers = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        _followServiceMock
            .Setup(m => m.GetFollowsAsync(userId))
            .ReturnsAsync(Result<Guid[]>.Success(followedUsers));

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                new Claim("UserId", userId.ToString())
                }, "TestAuth"))
            }
        };

        // Act
        var result = await _sut.GetFollows();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGuids = Assert.IsType<Guid[]>(okResult.Value);
        Assert.Equal(3, returnedGuids.Length);
        Assert.Equal(followedUsers, returnedGuids);
    }

    [Fact]
    public async Task GetFollowsAsync_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _sut.GetFollows();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetFollows_WithInvalidGuidInToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserId", "not-a-valid-guid")
                }, "TestAuth"))
            }
        };

        // Act
        var result = await _sut.GetFollows();

        // Assert 
        Assert.IsType<UnauthorizedResult>(result);

    }


    [Fact]
    public async Task GetFollows_UserFollowsNobody_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim("UserId", userId.ToString())
        }, "TestAuth"))
            }
        };

        _followServiceMock
            .Setup(s => s.GetFollowsAsync(userId))
            .ReturnsAsync(Result<Guid[]>.Success(Array.Empty<Guid>()));

        // Act
        var result = await _sut.GetFollows();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<Guid[]>(okResult.Value);
        Assert.Empty(data);
    }
}




