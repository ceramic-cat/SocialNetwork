using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("search")]
        [Authorize] 
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var users = await _userService.SearchUsersAsync(query);

            var result = users.Select(u => new
            {
                u.Id,
                u.Username
            });

            return Ok(result);
        }
    }
}
