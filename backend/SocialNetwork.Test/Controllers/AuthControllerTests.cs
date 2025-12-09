using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Entity.Models;
using System.Security.Claims;

namespace SocialNetwork.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_OnSuccess()
        {
            var request = new RegisterRequest { Username = "user", Password = "Pass123!", Email = "user@example.com" };
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(true);

            var result = await _authController.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Registration successful.", okResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_OnFailure()
        {
            var request = new RegisterRequest { Username = "user", Password = "Pass123!", Email = "user@example.com" };
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(false);

            var result = await _authController.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(AuthErrors.RegisterFailed, badRequest.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WithToken_OnSuccess()
        {
            var request = new LoginRequest { Username = "user", Password = "Pass123!" };
            _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync("token");

            var result = await _authController.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.Equal("token", loginResponse.Token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_OnFailure()
        {
            var request = new LoginRequest { Username = "user", Password = "wrong123!" };
            _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync((string?)null);

            var result = await _authController.Login(request);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(AuthErrors.InvalidCredentials, unauthorized.Value);
        }

         [Fact]
        public async Task EditProfile_ReturnsOk_OnSuccess()
        {
            var userId = Guid.NewGuid();
            var request = new EditProfileRequest { Username = "newuser" };
            _authServiceMock.Setup(s => s.EditProfileAsync(userId, request)).ReturnsAsync(true);

            // Mock User Claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _authController.EditProfile(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Profile updated successfully.", okResult.Value);
        }

        [Fact]
        public async Task EditProfile_ReturnsBadRequest_OnFailure()
        {
            var userId = Guid.NewGuid();
            var request = new EditProfileRequest { Username = "newuser" };
            _authServiceMock.Setup(s => s.EditProfileAsync(userId, request)).ReturnsAsync(false);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _authController.EditProfile(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(AuthErrors.CouldNotUpdateProfile, badRequest.Value);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsOk_OnSuccess()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.DeleteAccountAsync(userId)).ReturnsAsync(true);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _authController.DeleteAccount();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Account deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsBadRequest_OnFailure()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.DeleteAccountAsync(userId)).ReturnsAsync(false);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _authController.DeleteAccount();

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(AuthErrors.CouldNotDeleteAccount, badRequest.Value);
        }

        [Fact]
        public async Task DeleteUserById_ReturnsOk_OnSuccess()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.DeleteAccountAsync(userId)).ReturnsAsync(true);

            var result = await _authController.DeleteUserById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task DeleteUserById_ReturnsNotFound_OnFailure()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.DeleteAccountAsync(userId)).ReturnsAsync(false);

            var result = await _authController.DeleteUserById(userId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(AuthErrors.CouldNotDeleteUser, notFound.Value);
        }

        [Fact]
        public async Task GetUsernameById_ReturnsNotFound_WithEmptyGuid()
        {
            // Arrange
            var userId = Guid.Empty;
            _authServiceMock.Setup(s => s.GetUsernameAsync(userId)).ReturnsAsync((string?)null);

            // Act
            var result = await _authController.GetUsernameById(userId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(AuthErrors.UserNotFound, notFound.Value);
        }

        [Fact]
        public async Task GetUsernameById_ReturnNotFound_OnNoMatch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.GetUsernameAsync(userId)).ReturnsAsync((string?)null);

            // Act
            var result = await _authController.GetUsernameById(userId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(AuthErrors.UserNotFound, notFound.Value);
        }

        [Fact]
        public async Task GetUsernameById_ReturnOk_OnMatch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.GetUsernameAsync(userId)).ReturnsAsync("testuser");

            // Act
            var result = await _authController.GetUsernameById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            var username = value?.GetType().GetProperty("username")?.GetValue(value);
            Assert.Equal("testuser", username);
        }
    }
}