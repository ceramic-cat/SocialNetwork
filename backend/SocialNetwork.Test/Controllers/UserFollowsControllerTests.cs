using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Test.Controllers;

public class UserFollowsControllerTests
{

    private readonly Mock<IUserFollowsService> _followsUserServiceMock;
    private readonly UserFollowsController _sut;
    public UserFollowsControllerTests()
    {
        _followsUserServiceMock = new Mock<IUserFollowsService>();
         _sut = new UserFollowsController( _followsUserServiceMock.Object );
    }

    [Fact]
    public async Task FollowUser_ValidRequest_ReturnsOk()
    {
        //Arrange
        _followsUserServiceMock.Setup( m => 
        m.FollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Success);
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        //Act

        var reply = await _sut.Follow(followerId, followeeId);

        // Assert
        Assert.IsType<OkResult>(reply);
        _followsUserServiceMock.Verify(
            s =>s.FollowAsync(followerId, followeeId), Times.Once());
    }


}
