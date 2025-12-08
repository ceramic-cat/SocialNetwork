namespace SocialNetwork.Test.Services
{
    public class AuthServiceTests
    {
        private static Mock<DbSet<User>> CreateMockDbSet(List<User> users)
        {
            var mockSet = users.BuildMockDbSet();

            mockSet.Setup(m => m.Remove(It.IsAny<User>()))
                .Callback<User>(u => users.Remove(u));

            mockSet.Setup(m => m.Update(It.IsAny<User>()))
                .Callback<User>(u => { /* no-op */ });

            mockSet.Setup(m => m.Add(It.IsAny<User>()))
                .Callback<User>(u => users.Add(u));

            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) =>
                {
                    var id = (Guid)ids[0];
                    return users.FirstOrDefault(u => u.Id == id);
                });

            return mockSet;
        }

        private static IConfiguration CreateTestConfig()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
            { "Jwt:Key", "TestKey123456789012345678901234567890" },
            { "Jwt:Issuer", "testissuer" },
            { "Jwt:Audience", "testaudience" }
                })
                .Build();
        }

        private AuthService CreateMockedService(List<User> users)
        {
            var mockSet = CreateMockDbSet(users);
            var mockContext = new Mock<SocialNetworkDbContext>(new DbContextOptions<SocialNetworkDbContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            return new AuthService(mockContext.Object, CreateTestConfig());
        }

           [Fact]
        public async Task RegisterAsync_AddsUser_WhenUsernameIsUnique()
        {
            var users = new List<User>();
            var service = CreateMockedService(users);
            var request = new RegisterRequest { Username = "user", Password = "Pass123!", Email = "user@example.com" };

            var result = await service.RegisterAsync(request);

            Assert.True(result);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFalse_WhenUsernameExists()
        {
            var user = new User { Username = "duplicate", Email = "duplicate@example.com" };
            var users = new List<User> { user };
            var service = CreateMockedService(users);
            var request = new RegisterRequest { Username = "duplicate", Password = "Pass123!", Email = "duplicate@example.com" };

            var result = await service.RegisterAsync(request);

            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Pass123!");
            var user = new User { Id = Guid.NewGuid(), Username = "user", Password = hashedPassword, Email = "user@example.com" };
            var users = new List<User> { user };
            var service = CreateMockedService(users);

            var login = new LoginRequest { Username = "user", Password = "Pass123!" };
            var token = await service.LoginAsync(login);

            Assert.NotNull(token);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenCredentialsAreInvalid()
        {
            var users = new List<User>();
            var service = CreateMockedService(users);

            var login = new LoginRequest { Username = "nouser", Password = "nopass123!" };
            var token = await service.LoginAsync(login);

            Assert.Null(token);
        }


        [Fact]
        public async Task DeleteAccountAsync_RemovesUser_WhenUserExists()
        {
            var user = new User { Id = Guid.NewGuid(), Username = "user" };
            var users = new List<User> { user };
            var service = CreateMockedService(users);

            var result = await service.DeleteAccountAsync(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAccountAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            var users = new List<User>();
            var service = CreateMockedService(users);

            var result = await service.DeleteAccountAsync(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task EditProfileAsync_UpdatesUser_WhenUserExists()
        {
            var user = new User { Id = Guid.NewGuid(), Username = "user", Email = "old@example.com", Password = "oldpass" };
            var users = new List<User> { user };
            var service = CreateMockedService(users);

            var editRequest = new EditProfileRequest { Username = "updated", Email = "updated@example.com", Password = "newpass" };
            var result = await service.EditProfileAsync(user.Id, editRequest);

            Assert.True(result);
            Assert.Equal("updated", user.Username);
            Assert.Equal("updated@example.com", user.Email);
            Assert.True(BCrypt.Net.BCrypt.Verify("newpass", user.Password));
        }

        [Fact]
        public async Task EditProfileAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            var users = new List<User>();
            var service = CreateMockedService(users);

            var editRequest = new EditProfileRequest { Username = "updated" };
            var result = await service.EditProfileAsync(Guid.NewGuid(), editRequest);

            Assert.False(result);
        }
    }
}