using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly Services.GameService _game;
        public PlayerController(Services.GameService game) { _game = game; }

        [HttpGet("{playerId}/state")]
        public async Task<IActionResult> GetState(string playerId)
        {
            var state = await _game.GetPlayerStateAsync(playerId);
            if (state == null) return NotFound();
            return Ok(state);
        }

        [HttpPost("{playerId}/action")]
        public async Task<IActionResult> PostAction(string playerId, [FromBody] Models.ActionRequest req)
        {
            var result = await _game.ApplyActionAsync(playerId, req);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
