using Moq;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Interfaces;
using SocialNetwork.Repository.Services;
using Xunit;

namespace SocialNetwork.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _sut = new UserService(_userRepositoryMock.Object);
        }

        private static List<User> CreateSampleUsers() => new()
        {
            new User { Id = Guid.NewGuid(), Username = "Maja" },
            new User { Id = Guid.NewGuid(), Username = "Martin" }
        };

        [Fact]
        public async Task SearchUsersAsync_EmptyQuery_ReturnsEmptyList_AndDoesNotCallRepository()
        {
            // Arrange
            var query = "";

            // Act
            var result = await _sut.SearchUsersAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _userRepositoryMock.Verify(
                r => r.SearchByUsernameAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task SearchUsersAsync_TooShortQuery_ReturnsEmptyList_AndDoesNotCallRepository()
        {
            // Arrange
            var query = "a"; // bara 1 tecken

            // Act
            var result = await _sut.SearchUsersAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _userRepositoryMock.Verify(
                r => r.SearchByUsernameAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task SearchUsersAsync_ValidQuery_TrimsAndCallsRepository()
        {
            // Arrange
            var query = "  ma  ";
            var trimmed = "ma";

            var users = CreateSampleUsers();

            _userRepositoryMock
                .Setup(r => r.SearchByUsernameAsync(trimmed))
                .ReturnsAsync(users);

            // Act
            var result = await _sut.SearchUsersAsync(query);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Username == "Maja");
            Assert.Contains(result, u => u.Username == "Martin");

            _userRepositoryMock.Verify(
                r => r.SearchByUsernameAsync(trimmed),
                Times.Once);
        }
    }
}
