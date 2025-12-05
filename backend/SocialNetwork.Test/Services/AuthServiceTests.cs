using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Add this
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;
using Xunit;

namespace SocialNetwork.Test.Services
{
    public class AuthServiceTests
    {
        private AuthService CreateService(string dbName)
        {
            var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new SocialNetworkDbContext(options);

            // Mock IConfiguration with in-memory data for Jwt settings
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "TestKey123456789012345678901234567890" },
                    { "Jwt:Issuer", "testissuer" },
                    { "Jwt:Audience", "testaudience" }
                })
                .Build();

            return new AuthService(context, config);
        }

        [Fact]
        public async Task RegisterAsync_AddsUser_WhenUsernameIsUnique()
        {
            var service = CreateService(nameof(RegisterAsync_AddsUser_WhenUsernameIsUnique));
            var request = new RegisterRequest { Username = "user", Password = "Pass123!", Email = "user@example.com" };

            var result = await service.RegisterAsync(request);

            Assert.True(result);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFalse_WhenUsernameExists()
        {
            var service = CreateService(nameof(RegisterAsync_ReturnsFalse_WhenUsernameExists));
            var request = new RegisterRequest { Username = "duplicate", Password = "Pass123!", Email = "duplicate@example.com" };
            await service.RegisterAsync(request);

            var result = await service.RegisterAsync(request);

            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
        {
            var service = CreateService(nameof(LoginAsync_ReturnsToken_WhenCredentialsAreValid));
            var register = new RegisterRequest { Username = "user", Password = "Pass123!", Email = "user@example.com" };
            await service.RegisterAsync(register);

            var login = new LoginRequest { Username = "user", Password = "Pass123!" };
            var token = await service.LoginAsync(login);

            Assert.NotNull(token);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenCredentialsAreInvalid()
        {
            var service = CreateService(nameof(LoginAsync_ReturnsNull_WhenCredentialsAreInvalid));
            var login = new LoginRequest { Username = "nouser", Password = "nopass123!" };

            var token = await service.LoginAsync(login);

            Assert.Null(token);
        }
    }
}