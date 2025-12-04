using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Repository.Services;
using SocialNetwork.Entity.Models;
using Microsoft.AspNetCore.Authorization;

namespace SocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result)
                return BadRequest("Could not register.");
            return Ok("Registration successful.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);
            if (token == null)
                return Unauthorized("Invalid credentials.");
            return Ok(new LoginResponse { Token = token });
        }

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout successful.");
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var username = User.Identity?.Name;
    
             return Ok(new { 
                 Valid = true, 
                 UserId = userId,
                    Username = username 
                });
        }
    }
}