using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using Game.Api.Models;
using Game.Api.Storage.Repositories;
using Game.Api.Storage.Entities;

namespace Game.Api.Combat
{
    // Combat engine now supports data-driven weapons/ships and multi-turn sessions persisted in Azure Table Storage.
    public class CombatEngine
    {
        private readonly Random _rng;
        private readonly WeaponsTable _weaponsTable;
        private readonly ShipsTable _shipsTable;
        private readonly CombatSessionRepository _sessionRepo;

        public CombatEngine(WeaponsTable weaponsTable, ShipsTable shipsTable, CombatSessionRepository sessionRepo, int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
            _weaponsTable = weaponsTable;
            _shipsTable = shipsTable;
            _sessionRepo = sessionRepo;
        }

        // Start a new combat session (async because it persists)
        public async Task<CombatSession> StartSessionAsync(string sessionId, ShipState player, ShipState npc, CombatConfig config)
        {
            var session = new CombatSession
            {
                SessionId = sessionId,
                Player = player,
                Enemy = npc,
                Config = config,
                Turn = 0,
                ThreatClock = 0,
                Messages = new List<string>()
            };

            await _sessionRepo.UpsertAsync(session);
            return session;
        }

        public async Task<CombatTurnResult> StepSessionAsync(string sessionId, CombatAction playerAction)
        {
            var session = await _sessionRepo.GetAsync(sessionId);
            if (session == null) throw new InvalidOperationException("Session not found");

            session.Turn++;
            var player = session.Player.Clone();
            var npc = session.Enemy.Clone();
            var messages = new List<string>();
            int threatDelta = 0;

            // Resolve player action first
            var playerResult = ResolveAction(playerAction, player, npc, session.Config);
            messages.AddRange(playerResult.Messages);
            threatDelta += playerResult.ThreatDelta;

            // If NPC destroyed or disabled, skip NPC action
            if (!npc.IsDestroyed && !npc.IsDisabled)
            {
                var npcAction = DecideNpcAction(npc, player);
                var npcResult = ResolveAction(npcAction, npc, player, session.Config);
                messages.AddRange(npcResult.Messages.Select(m => "ENEMY: " + m));
                threatDelta += npcResult.ThreatDelta;
            }

            // End of turn: shield regen & capacitor regen
            ApplyEndOfTurnRegen(player, session.Config);
            ApplyEndOfTurnRegen(npc, session.Config);

            // Apply threat delta
            session.ThreatClock = Math.Min(6, Math.Max(0, session.ThreatClock + threatDelta));

            // Check for death/disable states
            var winner = DetermineWinner(player, npc);

            // persist updated states
            session.Player = player;
            session.Enemy = npc;
            session.Messages.AddRange(messages);
            session.LastUpdated = DateTime.UtcNow;

            await _sessionRepo.UpsertAsync(session);

            return new CombatTurnResult
            {
                PlayerShip = player,
                NpcShip = npc,
                Messages = messages,
                ThreatClockDelta = threatDelta,
                Winner = winner
            };
        }

        private static CombatAction DecideNpcAction(ShipState npc, ShipState player)
        {
            // naive: if shields down, try to target sensors or hail; else attack
            if (npc.ShieldsCurrent < npc.ShieldsMax * 0.2)
            {
                return CombatAction.TargetSensors;
            }
            return CombatAction.AttackPrimary;
        }

        private ActionResult ResolveAction(CombatAction action, ShipState actor, ShipState target, CombatConfig config)
        {
            var messages = new List<string>();
            int threatDelta = 0;

            switch (action)
            {
                case CombatAction.AttackPrimary:
                    var weapon = actor.PrimaryWeapon ?? actor.FallbackWeapon();
                    if (weapon == null)
                    {
                        messages.Add("No weapon to attack with.");
                        return new ActionResult { Messages = messages, ThreatDelta = 0 };
                    }

                    var rangeModifier = config.RangeModifierFor(action, actor, target);
                    var attackerSkill = actor.GetEffectiveSkillMultiplier("weapons");
                    var targetEvasion = target.GetEvasion();
                    var hitChance = Math.Clamp(weapon.BaseAccuracy * attackerSkill * rangeModifier * (1 - targetEvasion), 0.05, 0.98);

                    var roll = _rng.NextDouble();
                    messages.Add($"Firing {weapon.Name} (hitChance={hitChance:F2}, roll={roll:F2})");
                    if (roll <= hitChance)
                    {
                        var variance = 1.0 + (_rng.NextDouble() * 0.2 - 0.1);
                        var rawDamage = weapon.BaseDamage * variance;

                        var shieldMult = config.GetShieldMultiplier(weapon.Type);
                        var armorMult = config.GetArmorMultiplier(weapon.Type);

                        var shieldDamage = rawDamage * shieldMult;
                        if (target.ShieldsCurrent >= shieldDamage)
                        {
                            target.ShieldsCurrent -= shieldDamage;
                            messages.Add($"Shields absorbed {shieldDamage:F1} dmg. (shields left {target.ShieldsCurrent:F1})");
                        }
                        else
                        {
                            var leftover = shieldDamage - target.ShieldsCurrent;
                            target.ShieldsCurrent = 0;
                            var armorDamage = leftover * armorMult;
                            target.ArmorCurrent -= armorDamage;
                            messages.Add($"Shields depleted; armor took {armorDamage:F1} dmg (armor left {target.ArmorCurrent:F1})");
                            if (target.ArmorCurrent <= 0)
                            {
                                target.IsDestroyed = true;
                                messages.Add("Target armor failed — target destroyed/disabled.");
                            }
                        }

                        // weapons may advance threat
                        threatDelta += weapon.ThreatOnUse;
                    }
                    else
                    {
                        messages.Add("Missed.");
                        threatDelta += 0; // maybe missing advances clock less or same
                    }

                    break;
                case CombatAction.AttemptHail:
                    // low odds to pacify/delay
                    var hailRoll = _rng.NextDouble();
                    messages.Add($"Attempting hail (roll={hailRoll:F2})");
                    if (hailRoll < 0.25)
                    {
                        messages.Add("Hail successful: enemy stands down briefly.");
                        threatDelta -= 1;
                    }
                    else
                    {
                        messages.Add("Hail failed; enemy remains hostile.");
                        threatDelta += 1;
                    }
                    break;
                case CombatAction.DivertPowerToShields:
                    var shieldBoost = Math.Min(actor.ShieldsMax - actor.ShieldsCurrent, actor.CapacitorCurrent * 0.15);
                    actor.ShieldsCurrent += shieldBoost;
                    actor.CapacitorCurrent -= shieldBoost * 0.5; // cost
                    messages.Add($"Diverted power: shields +{shieldBoost:F1}, capacitor -{shieldBoost * 0.5:F1}");
                    threatDelta += 0;
                    break;
                case CombatAction.ManeuverToFar:
                    // increase evasion but risk hit
                    var maneuverRoll = _rng.NextDouble();
                    messages.Add($"Maneuvering to far range (roll={maneuverRoll:F2})");
                    if (maneuverRoll < 0.9)
                    {
                        actor.TempEvasion += 0.15;
                        messages.Add("Maneuver successful: evasion increased this turn.");
                        threatDelta += 0;
                    }
                    else
                    {
                        messages.Add("Maneuver risky: got hit during maneuver.");
                        threatDelta += 1;
                    }
                    break;
                case CombatAction.TargetSensors:
                    var targetRoll = _rng.NextDouble();
                    messages.Add($"Targeting enemy sensors (roll={targetRoll:F2})");
                    if (targetRoll < 0.35)
                    {
                        target.SensorsDisabledTurns = Math.Max(target.SensorsDisabledTurns, 2);
                        messages.Add("Sensors disabled for 2 turns.");
                        threatDelta -= 0;
                    }
                    else
                    {
                        messages.Add("Targeting failed.");
                        threatDelta += 1;
                    }
                    break;
                default:
                    messages.Add("Unrecognized action.");
                    break;
            }

            return new ActionResult { Messages = messages, ThreatDelta = threatDelta };
        }

        private void ApplyEndOfTurnRegen(ShipState ship, CombatConfig config)
        {
            // simple capacitor regen
            var capRegen = config.BaseCapRegenPerTurn;
            ship.CapacitorCurrent = Math.Min(ship.CapacitorMax, ship.CapacitorCurrent + capRegen);

            // shields regen if capacitor above threshold
            if (ship.CapacitorCurrent >= config.ShieldRechargeCapThreshold)
            {
                var rechargeAmount = Math.Min(ship.ShieldsMax - ship.ShieldsCurrent, ship.CapacitorCurrent * config.ShieldRechargeRate);
                ship.ShieldsCurrent += rechargeAmount;
                ship.CapacitorCurrent -= rechargeAmount * config.ShieldRechargeEnergyCost;
            }

            // decrement sensors disabled
            if (ship.SensorsDisabledTurns > 0) ship.SensorsDisabledTurns--;

            // reset temp evasion
            ship.TempEvasion = 0;
        }

        private CombatWinner DetermineWinner(ShipState a, ShipState b)
        {
            if (a.IsDestroyed && b.IsDestroyed) return CombatWinner.Draw;
            if (a.IsDestroyed) return CombatWinner.Enemy;
            if (b.IsDestroyed) return CombatWinner.Player;
            return CombatWinner.None;
        }
    }
}
