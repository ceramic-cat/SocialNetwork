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
    private void SetupUserContext(Guid followerId)
    {
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
    }
    [Fact]
    public async Task FollowAsync_ValidTokenAndGuid_ReturnsOk()
    {
        //Arrange
        var followeeId = Guid.NewGuid();
        var followerId = Guid.NewGuid();

        _followServiceMock.Setup(m =>
        m.FollowAsync(followerId, followeeId)).ReturnsAsync(Result.Success);
        SetupUserContext(followerId);

                //Act
                var reply = await _sut.FollowAsync(followeeId);

        // Assert
        Assert.IsType<OkResult>(reply);
        _followServiceMock.Verify(
            s => s.FollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task FollowAsync_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        SetupUserContext(followerId);


        _followServiceMock
            .Setup(m => m.FollowAsync(followerId, followeeId))
            .ReturnsAsync(Result.Failure("Error"));

        // Act
        var result = await _sut.FollowAsync(followeeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error", badRequestResult.Value);
        _followServiceMock.Verify(
            s => s.FollowAsync(followerId, followeeId), Times.Once());
    }


    [Fact]
    public async Task FollowAsync_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _sut.FollowAsync(Guid.NewGuid());

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }


    [Fact]
    public async Task FollowAsync_WithInvalidGuidInToken_ReturnsUnauthorized()
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
        var result = await _sut.FollowAsync(Guid.NewGuid());

        // Assert 
        Assert.IsType<UnauthorizedResult>(result);

    }

    [Fact]
    public async Task UnfollowAsync_ValidRequest_ReturnsOk()
    {
        _followServiceMock.Setup(m =>
            m.UnfollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Success);
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        SetupUserContext(followerId);

        //Act
        var response = await _sut.UnfollowAsync(followeeId);

        // Assert
        Assert.IsType<OkResult>(response);
        _followServiceMock.Verify(
            s => s.UnfollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task UnfollowAsync_InvalidRequest_ReturnsBadRequest()
    {
        _followServiceMock.Setup(m =>
            m.UnfollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Failure("Error"));
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        SetupUserContext(followerId);

        //Act
        var response = await _sut.UnfollowAsync(followeeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Error", badRequestResult.Value);
        _followServiceMock.Verify(
            s => s.UnfollowAsync(followerId, followeeId), Times.Once());
    }

    [Fact]
    public async Task UnfollowAsync_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _sut.UnfollowAsync(Guid.NewGuid());

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UnfollowAsync_WithInvalidGuidInToken_ReturnsUnauthorized()
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
        var result = await _sut.UnfollowAsync(Guid.NewGuid());

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

        SetupUserContext(userId);

        // Act
        var result = await _sut.GetFollowsAsync();

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
        var result = await _sut.GetFollowsAsync();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetFollowsAsync_WithInvalidGuidInToken_ReturnsUnauthorized()
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
        var result = await _sut.GetFollowsAsync();

        // Assert 
        Assert.IsType<UnauthorizedResult>(result);

    }


    [Fact]
    public async Task GetFollowsAsync_UserFollowsNobody_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);


        _followServiceMock
            .Setup(s => s.GetFollowsAsync(userId))
            .ReturnsAsync(Result<Guid[]>.Success(Array.Empty<Guid>()));

        // Act
        var result = await _sut.GetFollowsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<Guid[]>(okResult.Value);
        Assert.Empty(data);
    }

    [Fact]
    public async Task IsFollowingAsync_FollowerFollowsFollowee_ReturnsOKTrue()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        SetupUserContext(followerId);

        _followServiceMock.Setup(s => s.IsFollowingAsync(followerId, followeeId))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _sut.IsFollowingAsync(followeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<bool>(okResult.Value);
        Assert.True(data);
    }

    [Fact]
    public async Task IsFollowingAsync_FollowerNotFollowingFollowee_ReturnsOKFalse()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        SetupUserContext(followerId);

        _followServiceMock.Setup(s => s.IsFollowingAsync(followerId, followeeId))
            .ReturnsAsync(Result<bool>.Success(false));

        // Act
        var result = await _sut.IsFollowingAsync(followeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<bool>(okResult.Value);
        Assert.False(data);
    }

    [Fact]
    public async Task IsFollowingAsync_WithEmptyFolloweeGuid_ReturnsBadRequest()
    {        // Arrange
        var followerId = Guid.NewGuid();
        SetupUserContext(followerId);

        // Act
        var result = await _sut.IsFollowingAsync(Guid.Empty);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);

    }

    [Fact]
    public async Task IsFollowingAsync_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _sut.IsFollowingAsync(Guid.NewGuid());

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task IsFollowingAsync_WithInvalidGuidInToken_ReturnsUnauthorized()
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
        var result = await _sut.IsFollowingAsync(Guid.NewGuid());

        // Assert 
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetFollowsInfo_ValidUser_ReturnsOkWithUsers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var followedUsers = new[] {
            new FollowedUserDto { Id = Guid.NewGuid(), Username = "alice" },
            new FollowedUserDto { Id = Guid.NewGuid(), Username = "bob" }
        };
        var followerId = Guid.NewGuid();

        SetupUserContext(followerId);

        _followServiceMock
            .Setup(s => s.GetFollowsWithUserInfoAsync(userId))
            .ReturnsAsync(Result<FollowedUserDto[]>.Success(followedUsers));

        // Act
        var result = await _sut.GetFollowsInfo();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<FollowedUserDto[]>(okResult.Value);
        Assert.Equal(2, data.Length);
    }
    [Fact]
    public async Task GetFollowsInfo_ValidUser_ReturnsCorrectUsernames()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followedUsers = new[]
        {
        new FollowedUserDto { Id = Guid.NewGuid(), Username = "alice" },
        new FollowedUserDto { Id = Guid.NewGuid(), Username = "bob" }
        };

        SetupUserContext(followerId);

        _followServiceMock
            .Setup(s => s.GetFollowsWithUserInfoAsync(followerId))
            .ReturnsAsync(Result<FollowedUserDto[]>.Success(followedUsers));

        // Act
        var result = await _sut.GetFollowsInfo();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<FollowedUserDto[]>(okResult.Value);
        Assert.Contains(data, u => u.Username == "alice");
        Assert.Contains(data, u => u.Username == "bob");
    }

    [Fact]
    public async Task GetFollowsInfo_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _sut.GetFollowsInfo();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
    [Fact]
    public async Task GetFollowsInfo_InvalidGuidInToken_ReturnsUnauthorized()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, "not-a-guid")
            }, "TestAuth"))
            }
        };

        // Act
        var result = await _sut.GetFollowsInfo();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }


    [Fact]
    public async Task GetFollowsInfo_UserFollowsNobody_ReturnsEmptyArray()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        SetupUserContext(followerId);
        _followServiceMock
            .Setup(s => s.GetFollowsWithUserInfoAsync(followerId))
            .ReturnsAsync(Result<FollowedUserDto[]>.Success(Array.Empty<FollowedUserDto>()));

        // Act
        var result = await _sut.GetFollowsInfo();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<FollowedUserDto[]>(okResult.Value);
        Assert.Empty(data);
    }


}





