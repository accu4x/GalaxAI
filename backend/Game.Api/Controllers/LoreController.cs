using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/lore")]
    public class LoreController : ControllerBase
    {
        // GET api/lore/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLore(string id)
        {
            // TODO: fetch from Azure Blob Storage
            var markdown = "# Placeholder Lore\n\nThis is a placeholder lore file.";
            return Ok(new { id, markdown });
        }
    }
}
