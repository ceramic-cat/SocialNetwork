using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialNetwork.API.Controllers;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.Test.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _sut;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _sut = new UsersController(_userServiceMock.Object);
        }

        private static List<User> CreateSampleUsers() => new()
        {
            new User { Id = Guid.NewGuid(), Username = "Maja" },
            new User { Id = Guid.NewGuid(), Username = "Martin" }
        };


        [Fact]
        public async Task Search_ValidQuery_ReturnsOkWithUsers()
        {
            // Arrange

            var query = "ma";
            var users = CreateSampleUsers();

            _userServiceMock
                .Setup(s => s.SearchUsersAsync(query))
                .ReturnsAsync(users);

            // Act
            var result = await _sut.Search(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(2, body.Count());

            _userServiceMock.Verify(
                s => s.SearchUsersAsync(query),
                Times.Once);
        }

        [Fact]
        public async Task Search_EmptyQuery_ReturnsOkWithEmptyList()
        {
            // Arrange
            var query = string.Empty;

            _userServiceMock
                .Setup(s => s.SearchUsersAsync(query))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _sut.Search(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Empty(body);

            _userServiceMock.Verify(
                s => s.SearchUsersAsync(query),
                Times.Once);
        }
    }
}
