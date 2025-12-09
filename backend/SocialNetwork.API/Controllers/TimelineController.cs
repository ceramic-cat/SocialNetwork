using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/timeline")]
    [Authorize]

    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _timelineService;
        private readonly SocialNetworkDbContext _db;

        public TimelineController(ITimelineService timelineService, SocialNetworkDbContext db)
        {
            _timelineService = timelineService;
            _db = db;
        }

        [HttpGet]
        public async Task <IActionResult> GetTimeline (Guid userId)
        {
            var posts = await _timelineService.GetPostsByUserIdAsync(userId);

            var userIds = posts
              .SelectMany(p => new[] { p.SenderId, p.ReceiverId })
              .Distinct()
              .ToList();

            var users = await _db.Users
              .Where(u => userIds.Contains(u.Id))
              .ToDictionaryAsync(u => u.Id, u => u.Username);

            var dtos = posts.Select(p => new PostDto
            {
                Id = p.Id,
                SenderId = p.SenderId,
                SenderUsername = users.TryGetValue(p.SenderId, out var sName) ? sName : string.Empty,
                ReceiverId = p.ReceiverId,
                ReceiverUsername = users.TryGetValue(p.ReceiverId, out var rName) ? rName : string.Empty,
                Content = p.Content,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(dtos);
        }
    }
}
