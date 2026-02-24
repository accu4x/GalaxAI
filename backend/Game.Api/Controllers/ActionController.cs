using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Game.Api.DTOs;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/action")]
    public class ActionController : ControllerBase
    {
        // POST api/action
        [HttpPost]
        public async Task<IActionResult> PostAction([FromBody] ActionRequestDto req)
        {
            // TODO: validate API key, apply action via storage adapter, return result
            var result = new ActionResultDto
            {
                Success = true,
                ResultText = $"Action '{req.ActionId}' executed for player {req.PlayerId}.",
                StateDelta = null
            };
            return Ok(result);
        }
    }
}
