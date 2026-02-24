using Microsoft.AspNetCore.Mvc;
using Game.Api.Combat;
using Game.Api.Models;
using System.Text.Json;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CombatController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] CombatRequestDto dto)
        {
            // minimal demo: build simple ship states and run engine
            var engine = new CombatEngine(1234);

            var playerShip = new ShipState
            {
                ShipId = dto.PlayerShipId ?? "player-ship",
                ShieldsMax = 100,
                ShieldsCurrent = 70,
                ArmorMax = 100,
                ArmorCurrent = 100,
                CapacitorMax = 50,
                CapacitorCurrent = 40,
                PrimaryWeapon = new WeaponState { Name = "Micron Laser", Type = "Beam", BaseAccuracy = 0.8, BaseDamage = 18, Ammo = -1 },
                BaseEvasion = 0.12
            };

            var npcShip = new ShipState
            {
                ShipId = dto.NpcShipId ?? "npc-ship",
                ShieldsMax = 80,
                ShieldsCurrent = 55,
                ArmorMax = 80,
                ArmorCurrent = 80,
                CapacitorMax = 30,
                CapacitorCurrent = 25,
                PrimaryWeapon = new WeaponState { Name = "Drone Pea Shooter", Type = "Kinetic", BaseAccuracy = 0.6, BaseDamage = 12, Ammo = 30 },
                BaseEvasion = 0.05
            };

            var config = new CombatConfig();
            var action = dto.Action switch
            {
                "hail" => CombatAction.AttemptHail,
                "shields" => CombatAction.DivertPowerToShields,
                "maneuver" => CombatAction.ManeuverToFar,
                "sensors" => CombatAction.TargetSensors,
                _ => CombatAction.AttackPrimary,
            };

            var res = engine.ResolveTurn(new CombatTurnRequest
            {
                PlayerShip = playerShip,
                NpcShip = npcShip,
                PlayerAction = action,
                Config = config
            });

            return Ok(new CombatResponseDto { ResultJson = JsonSerializer.Serialize(res) });
        }
    }
}
