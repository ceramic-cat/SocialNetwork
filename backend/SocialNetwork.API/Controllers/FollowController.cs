using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Entity.Models.Follow;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FollowController : ControllerBase
{
    private readonly IFollowsService _userFollowsService;

    public FollowController(IFollowsService userFollowsService)
    {
        _userFollowsService = userFollowsService;
    }

    [HttpPost("follow")]
    public async Task<IActionResult> Follow(FollowRequest request)
    {
        throw new NotImplementedException();
    }


}
