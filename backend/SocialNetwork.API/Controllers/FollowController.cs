using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace SocialNetwork.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FollowController : ControllerBase
{
    private readonly IFollowsService _followService;

    public FollowController(IFollowsService followsService)
    {
        _followService = followsService;
    }
    [Authorize]
    [HttpPost(":followeeId")]
    public async Task<IActionResult> Follow(Guid followeeId)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var followerId))
        {
            return Unauthorized();
        }

        var result = await _followService.FollowAsync(
            followerId,
            followeeId
            );

        if (result.IsSuccess == true)
        { return Ok(); }

        return BadRequest(result.ErrorMessage);

    }
    [Authorize]
    [HttpDelete(":followeeId")]
    public async Task<IActionResult> Unfollow(Guid followeeId)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var followerId))
        {
            return Unauthorized();
        }

        var result = await _followService.UnfollowAsync(followerId, followeeId);

        if (result.IsSuccess == true)
        { return Ok(); }
        return BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetFollows()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _followService.GetFollowsAsync(userId);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.ErrorMessage);
    }
}
