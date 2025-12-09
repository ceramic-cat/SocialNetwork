using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.API.Models;
using SocialNetwork.Repository.Errors;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers;

[ApiController]
[Authorize]
[Route("api/dm")]
public class DirectMessageController : ControllerBase
{
    private readonly IDirectMessageService _directMessageService;

    public DirectMessageController(IDirectMessageService directMessageService)
    {
        _directMessageService = directMessageService;
    }

    [HttpPost]
    public async Task<IActionResult> SendDirectMessage([FromBody] CreateDirectMessageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var senderId))
        {
            var authFail = Result.Failure(DirectMessageErrors.InvalidUserId);
            return Unauthorized(authFail);
        }

        var result = await _directMessageService.SendDirectMessageAsync(
            senderId,
            request.ReceiverId,
            request.Content
        );

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
