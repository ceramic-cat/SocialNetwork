using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;
using System.Reflection.Metadata.Ecma335;

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
    [HttpGet]
    public async Task<IActionResult> GetFollows([FromBody] Guid id)
    {
        var result = await _followService.GetFollowsAsync(id);

        if (result.IsSuccess == true) { return Ok(result.Data); }

        return BadRequest(result.ErrorMessage);
    }
}
