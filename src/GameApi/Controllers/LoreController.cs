using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers
{
    [ApiController]
    [Route("api/lore")]
    public class LoreController : ControllerBase
    {
        [HttpGet("{loreId}")]
        public IActionResult GetLore(string loreId)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "lore", loreId + ".md");
            if (!System.IO.File.Exists(path)) return NotFound();
            var content = System.IO.File.ReadAllText(path);
            return Ok(new { id = loreId, content = content });
        }
    }
}
