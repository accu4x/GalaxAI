using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        [HttpPost("reload-scripts")]
        public IActionResult ReloadScripts()
        {
            // TODO: implement a signal to DialogRunner or data loader to re-read MD files
            _logger.LogInformation("/admin/reload-scripts called");
            return Ok(new { status = "reloaded_requested" });
        }
    }
}