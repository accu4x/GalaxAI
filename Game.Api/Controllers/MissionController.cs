using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Game.Api.Storage.Repositories;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/mission")]
    public class MissionController : ControllerBase
    {
        private readonly ILogger<MissionController> _logger;
        private readonly SystemEventRepository _eventRepo;

        public MissionController(ILogger<MissionController> logger, SystemEventRepository eventRepo)
        {
            _logger = logger;
            _eventRepo = eventRepo;
        }

        [HttpGet("newsfeed")]
        public async Task<IActionResult> GetNewsfeed()
        {
            // TODO: query SystemEvents to return recent news events for the in-game newsfeed
            return Ok(new { status = "not_implemented_yet" });
        }
    }
}