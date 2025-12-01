using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;
using Xunit;

namespace SocialNetwork.Test.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegisterAsync_AddsUser_WhenUsernameIsUnique()
        {
            var service = new AuthService();
            var request = new RegisterRequest { Username = "unique", Password = "Pass123!" };

            var result = await service.RegisterAsync(request);

            Assert.True(result);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFalse_WhenUsernameExists()
        {
            var service = new AuthService();
            var request = new RegisterRequest { Username = "duplicate", Password = "Pass123!" };
            await service.RegisterAsync(request);

            var result = await service.RegisterAsync(request);

            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
        {
            var service = new AuthService();
            var register = new RegisterRequest { Username = "user", Password = "Pass123!" };
            await service.RegisterAsync(register);

            var login = new LoginRequest { Username = "user", Password = "Pass123!" };
            var token = await service.LoginAsync(login);

            Assert.NotNull(token);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenCredentialsAreInvalid()
        {
            var service = new AuthService();
            var login = new LoginRequest { Username = "nouser", Password = "nopass123!" };

            var token = await service.LoginAsync(login);

            Assert.Null(token);
        }
    }
}