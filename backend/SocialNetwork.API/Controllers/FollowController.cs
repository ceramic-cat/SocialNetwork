using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace SocialNetwork.API.Controllers;

public interface IFollowController
{
    Task<IActionResult> FollowAsync(Guid followeeId);
    Task<IActionResult> GetFollowsAsync();
    Task<IActionResult> IsFollowingAsync(Guid followeeId);
    Task<IActionResult> UnfollowAsync(Guid followeeId);
}
/// <summary>
/// Handles users following other users and related information.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class FollowController : ControllerBase, IFollowController
{
    private readonly IFollowsService _followService;

    public FollowController(IFollowsService followsService)
    {
        _followService = followsService;
    }

    /// <summary>
    /// Adds a follower-follwee relationship to logged in user.
    /// </summary>
    /// <param name="followeeId">User guid to follow</param>
    [Authorize]
    [HttpPost("{followeeId}")]
    public async Task<IActionResult> FollowAsync(Guid followeeId)
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
    /// <summary>
    /// Logged in user unfollows followee.
    /// </summary>
    /// <param name="followeeId">User guid to unfollow</param>
    [Authorize]
    [HttpDelete("{followeeId}")]
    public async Task<IActionResult> UnfollowAsync(Guid followeeId)
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

    /// <summary>
    /// Get all users logged in user follows.
    /// </summary>
    /// <returns>Returns array of guid. If no follows returns empty array.</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetFollowsAsync()
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
    /// <summary>
    /// Check if there is a following relationship between logged in user and followeeId.
    /// </summary>
    /// <param name="followeeId">User id to check if logged in user follows.</param>
    /// <returns>Returns boolean</returns>
    [Authorize]
    [HttpGet("{followeeId}")]
    public async Task<IActionResult> IsFollowingAsync(Guid followeeId)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var followerId))
        {
            return Unauthorized();
        }

        if (followeeId == Guid.Empty)
        {
            return BadRequest("Empty user");
        }

        var result = await _followService.IsFollowingAsync(followerId, followeeId);
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<IActionResult> GetFollowsInfo()
    {
        throw new NotImplementedException();
    }
}
