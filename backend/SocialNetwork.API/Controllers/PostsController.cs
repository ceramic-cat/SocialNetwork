using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.API.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers
{
	[ApiController]
    [Authorize]
    [Route("api/[controller]")]
	public class PostsController : ControllerBase
	{
		private readonly IPostService _postService;
		public PostsController(IPostService postService)
		{
			_postService = postService;
		}
		[HttpPost]
		public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
		{
			if (!ModelState.IsValid)
			{
				return ValidationProblem(ModelState);
			}

			var userIdClaim = User.FindFirst("UserId")?.Value;

			if (!Guid.TryParse(userIdClaim, out var senderId))
			{
                var authFail = PostResult.Fail("Invalid or missing user id in token.");
                return Unauthorized(authFail);
            }

			var result = await _postService.CreatePostAsync(
				senderId,
				request.ReceiverId,
				request.Content);

			if (!result.Success)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdClaim, out var requesterId))
            {
                var authFail = PostResult.Fail("Invalid or missing user id in token.");
                return Unauthorized(authFail);
            }

            var result = await _postService.DeletePostAsync(postId, requesterId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}