using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Game.Api.DTOs;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        // GET api/player/{id}/state
        [HttpGet("{id}/state")]
        public async Task<IActionResult> GetState(string id)
        {
            // TODO: wire to storage adapter
            var state = new PlayerStateDto
            {
                PlayerId = id,
                Location = "Hangar Bay",
                Hp = 100,
                Inventory = new string[] { "Wrench", "Data Pad" }
            };
            return Ok(state);
        }
    }
}
