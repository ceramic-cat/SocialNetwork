using Microsoft.AspNetCore.Mvc;
using SocialNetwork.API.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers
{
	[ApiController]
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
			var result = await _postService.CreatePostAsync(
				request.SenderId,
				request.ReceiverId,
				request.Content);
			
			if (!result.Success)
			{
				return BadRequest(result.ErrorMessage);
			}
			return Ok();
		}
	}
}