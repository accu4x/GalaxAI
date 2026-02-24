using Microsoft.AspNetCore.Mvc;
using Game.Api.Models;
using System.Collections.Generic;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        // In-memory stub; replace with real datastore
        private static readonly Dictionary<string, PlayerSkillDto> _skills = new Dictionary<string, PlayerSkillDto>();

        [HttpGet("player/{playerId}")]
        public ActionResult<List<PlayerSkillDto>> GetPlayerSkills(string playerId)
        {
            // stub: return empty or seeded skills
            return Ok(new List<PlayerSkillDto>());
        }

        [HttpPost("player/{playerId}/use/{skillId}")]
        public ActionResult<PlayerSkillDto> UseSkill(string playerId, string skillId, [FromBody] UseSkillRequest req)
        {
            // stub: compute XP gains, return updated PlayerSkillDto
            return Ok(new PlayerSkillDto { SkillId = skillId, Tier = SkillTier.Basic, TierProgress = 5.0, TotalXP = 100, Specialty = null, Mastery = false });
        }

        [HttpPost("player/{playerId}/install-chip/{skillId}")]
        public ActionResult InstallChip(string playerId, string skillId, [FromBody] InstallChipRequest req)
        {
            // stub: validate chip owned, consume, promote if eligible
            return Ok(new { message = "chip installed (stub)" });
        }

        [HttpPost("player/{playerId}/choose-specialty/{skillId}")]
        public ActionResult ChooseSpecialty(string playerId, string skillId, [FromBody] ChooseSpecialtyRequest req)
        {
            // stub: validate availability and cost
            return Ok(new { message = "specialty chosen (stub)" });
        }
    }
}
