using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Repository.Services;
using SocialNetwork.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Repository.Errors;

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
                return BadRequest(AuthErrors.RegisterFailed);
            return Ok("Registration successful.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);
            if (token == null)
                return Unauthorized(AuthErrors.InvalidCredentials);
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
                return Unauthorized(AuthErrors.UserNotFound);

            var result = await _authService.EditProfileAsync(Guid.Parse(userId), request);
            if (!result)
                return BadRequest(AuthErrors.CouldNotUpdateProfile);

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
                return BadRequest(AuthErrors.CouldNotDeleteAccount);

            return Ok("Account deleted successfully.");
        }

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout successful.");
        }

        /// <summary>
        /// Get username that belongs to a user id.
        /// </summary>
        /// <param name="id">Guid for user</param>
        /// <returns>returns json with username</returns>
        [HttpGet("get-username/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUsernameById(Guid id)
        {
            var result = await _authService.GetUsernameAsync(id);
            if (result is null)
            {
                return NotFound(AuthErrors.UserNotFound);
            }
            return Ok(new {username = result});

        }
    }
}