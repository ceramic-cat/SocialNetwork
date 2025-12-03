using Microsoft.AspNetCore.Mvc;
using SocialNetwork.API.Models;
using SocialNetwork.Repository.Services;

namespace SocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/dm")]
    public class DMController : ControllerBase
    {
        private readonly IDirectMessageService _dmService;

        public DMController(IDirectMessageService dmService)
        {
            _dmService = dmService;
        }

        [HttpPost]
        public async Task<IActionResult> SendDM([FromBody] CreateDMRequest request)
        {
            var result = await _dmService.SendDirectMessageAsync(
                request.SenderId,
                request.ReceiverId,
                request.Message);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }
    }
}

