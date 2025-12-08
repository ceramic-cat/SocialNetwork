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

        

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var username = User.Identity?.Name;

            return Ok(new
            {
                Valid = true,
                UserId = userId,
                Username = username
            });
        }

        [HttpPut("edit-profile")]
        [Authorize]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
                return Unauthorized("User not found.");

            var result = await _authService.EditProfileAsync(Guid.Parse(userId), request);
            if (!result)
                return BadRequest("Could not update profile.");

            return Ok("Profile updated successfully.");
        }
        
        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
                return Unauthorized("User not found.");

            var result = await _authService.DeleteAccountAsync(Guid.Parse(userId));
            if (!result)
                return BadRequest("Could not delete account.");

            return Ok("Account deleted successfully.");
        }

        [HttpDelete("delete-user/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserById(Guid id)
        {
            var result = await _authService.DeleteAccountAsync(id);
            if (!result)
                return NotFound("User not found or could not be deleted.");

            return Ok("User deleted successfully.");
        }

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout successful.");
        }

        [HttpGet("get-username/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUsernameById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}