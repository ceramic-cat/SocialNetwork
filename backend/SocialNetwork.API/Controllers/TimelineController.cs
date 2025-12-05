using Microsoft.AspNetCore.Mvc;
using SocialNetwork.API.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/timeline")]

    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _timelineService;

        public TimelineController(ITimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        [HttpGet]
        public async Task <IActionResult> GetTimeline (Guid userId)
        {
            var posts = await _timelineService.GetPostsByUserIdAsync(userId);

            var dtos = posts.Select(p => new PostDto
            {
                Id = p.Id,
                SenderId = p.SenderId,
                ReceiverId = p.ReceiverId,
                Content = p.Content,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(dtos);
        }
    }
}
