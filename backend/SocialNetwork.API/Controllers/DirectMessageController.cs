using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Errors;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers;

[ApiController]
[Authorize]
[Route("api/dm")]
public class DirectMessageController : ControllerBase
{
    private readonly IDirectMessageService _directMessageService;
    private readonly SocialNetworkDbContext _db;

    public DirectMessageController(IDirectMessageService directMessageService, SocialNetworkDbContext db)
    {
        _directMessageService = directMessageService;
        _db = db;
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

    [HttpGet]
    public async Task<IActionResult> GetDirectMessages()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            var authFail = Result.Failure(DirectMessageErrors.InvalidUserId);
            return Unauthorized(authFail);
        }

        var messages = await _directMessageService.GetMessagesByUserIdAsync(userId);

        var userIds = messages
            .SelectMany(m => new[] { m.SenderId, m.ReceiverId })
            .Distinct()
            .ToList();

        var users = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Username);

        var dtos = messages.Select(m => new DirectMessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderUsername = users.TryGetValue(m.SenderId, out var sName) ? sName : string.Empty,
            ReceiverId = m.ReceiverId,
            ReceiverUsername = users.TryGetValue(m.ReceiverId, out var rName) ? rName : string.Empty,
            Content = m.Content,
            CreatedAt = m.CreatedAt
        }).ToList();

        return Ok(dtos);
    }
}
