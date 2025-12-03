using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserFollowsController : ControllerBase
{
    private readonly IUserFollowsService _userFollowsService;

    public UserFollowsController(IUserFollowsService userFollowsService)
    {
        _userFollowsService = userFollowsService;
    }

    [HttpPost("follow")]
    public async Task<IActionResult> Follow()
    {
        throw new NotImplementedException();
    }


}
