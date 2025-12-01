using SocialNetwork.Entity.Models;

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
            var request = new RegisterRequest { Username = "user", Password = "Pass123!" };
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(true);

            var result = await _authController.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Registration successful.", okResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_OnFailure()
        {
            var request = new RegisterRequest { Username = "user", Password = "Pass123!" };
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(false);

            var result = await _authController.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not register.", badRequest.Value);
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
            Assert.Equal("Invalid credentials.", unauthorized.Value);
        }
    }
}