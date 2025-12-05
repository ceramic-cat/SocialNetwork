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

    [HttpPost]
    public async Task<IActionResult> Follow(FollowRequest request)
    {
        var result = await _followService.FollowAsync(
            request.FollowerId, 
            request.FolloweeId
            );

        if (result.IsSuccess == true)
        { return Ok(); }

        return BadRequest(result.ErrorMessage);

    }
    [HttpDelete]
    public async Task<IActionResult> Unfollow(FollowRequest request)
    {
        var result = await _followService.UnfollowAsync(
            request.FollowerId, 
            request.FolloweeId
            );

        if (result.IsSuccess == true)
        { return Ok(); }
        return BadRequest(result.ErrorMessage);
    }
    [Authorize]
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetFollows()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token");
        }

        var result = await _followService.GetFollowsAsync(userId);

        if (result.IsSuccess == true) { return Ok(result.Data); }

        return BadRequest(result.ErrorMessage);
    }
}
