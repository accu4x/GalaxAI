using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Game.Api.Storage.Repositories;
using Game.Api.Models;
using Game.Api.Combat;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionController : ControllerBase
    {
        private readonly CombatSessionRepository _combatRepo;
        private readonly CombatEngine _combatEngine;

        public ActionController(CombatSessionRepository combatRepo, CombatEngine combatEngine)
        {
            _combatRepo = combatRepo;
            _combatEngine = combatEngine;
        }

        [HttpPost("combat/start")]
        public async Task<IActionResult> StartCombat([FromBody] StartCombatRequest req)
        {
            // create a new combat session from request
            var session = CombatSession.CreateFromRequest(req);
            await _combatRepo.UpsertAsync(session);
            return Ok(session);
        }

        [HttpPost("combat/{sessionId}/turn")]
        public async Task<IActionResult> CombatTurn(string sessionId, [FromBody] CombatTurnRequest req)
        {
            var session = await _combatRepo.GetAsync(sessionId);
            if (session == null) return NotFound();

            var result = _combatEngine.ResolveTurn(session, req);
            session.ApplyTurnResult(result);
            await _combatRepo.UpsertAsync(session);

            return Ok(result);
        }
    }
}
