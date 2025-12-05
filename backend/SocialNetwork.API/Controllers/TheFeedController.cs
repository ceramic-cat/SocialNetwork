using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Repository.Services;
using SocialNetwork.Repository.Interfaces;
using SocialNetwork.API.Models;
using SocialNetwork.Entity.Models;

namespace SocialNetwork.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class TheFeedController : ControllerBase
  {
      private readonly IFollowsService _followsService;
        private readonly IPostRepository _postRepository;

        public TheFeedController(IFollowsService followsService, IPostRepository postRepository)
        {
            _followsService = followsService;
            _postRepository = postRepository;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFeed(Guid userId)
        {
            var followsResult = await _followsService.GetFollowsAsync(userId);
            if (!followsResult.IsSuccess)
                return BadRequest(followsResult.ErrorMessage);

            var followedUserIds = followsResult.Data;

            if (followedUserIds == null)
                return Ok(new List<PostDto>());

            var posts = await _postRepository.GetPostsByUserIdsAsync(followedUserIds);

            var orderedPosts = posts.OrderByDescending(p => p.CreatedAt)
                                    .Select(p => new PostDto
                                    {
                                      Id = p.Id,
                                      SenderId = p.SenderId,
                                      ReceiverId = p.ReceiverId,
                                      Content = p.Content,
                                      CreatedAt = p.CreatedAt,
                                    });

            return Ok(orderedPosts);
        }
    }

}