using System.Collections.Generic;

namespace Game.Api.Combat
{
    public enum CombatAction
    {
        AttackPrimary,
        AttemptHail,
        DivertPowerToShields,
        ManeuverToFar,
        TargetSensors
    }

    public enum CombatWinner { None, Player, Enemy, Draw }

    public class CombatTurnRequest
    {
        public ShipState PlayerShip { get; set; }
        public ShipState NpcShip { get; set; }
        public CombatAction PlayerAction { get; set; }
        public CombatConfig Config { get; set; }
    }

    public class CombatTurnResult
    {
        public ShipState PlayerShip { get; set; }
        public ShipState NpcShip { get; set; }
        public List<string> Messages { get; set; }
        public int ThreatClockDelta { get; set; }
        public CombatWinner Winner { get; set; }
    }

    public class ShipState
    {
        public string ShipId { get; set; }
        public string OwnerId { get; set; }
        public string ShipType { get; set; }
        public double ShieldsMax { get; set; }
        public double ShieldsCurrent { get; set; }
        public double ArmorMax { get; set; }
        public double ArmorCurrent { get; set; }
        public double CapacitorMax { get; set; }
        public double CapacitorCurrent { get; set; }
        public WeaponState PrimaryWeapon { get; set; }
        public List<WeaponState> Weapons { get; set; } = new List<WeaponState>();
        public double TempEvasion { get; set; }
        public double BaseEvasion { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsDisabled { get; set; }
        public int SensorsDisabledTurns { get; set; }

        public ShipState Clone()
        {
            return (ShipState)MemberwiseClone();
        }

        public double GetEvasion()
        {
            return Math.Min(0.9, BaseEvasion + TempEvasion);
        }

        public double GetEffectiveSkillMultiplier(string skill)
        {
            // placeholder: real implementation will read from skill DTOs
            return 0.6; // default mid-skill
        }

        public WeaponState FallbackWeapon()
        {
            return Weapons != null && Weapons.Count > 0 ? Weapons[0] : null;
        }
    }

    public class WeaponState
    {
        public string Name { get; set; }
        public string Type { get; set; } // Beam, Kinetic, Missile, Pulse
        public double BaseAccuracy { get; set; }
        public double BaseDamage { get; set; }
        public int Ammo { get; set; }
        public int ThreatOnUse { get; set; } = 1;
    }

    public class CombatConfig
    {
        public double BaseCapRegenPerTurn { get; set; } = 5.0;
        public double ShieldRechargeCapThreshold { get; set; } = 5.0;
        public double ShieldRechargeRate { get; set; } = 0.05; // fraction of capacitor
        public double ShieldRechargeEnergyCost { get; set; } = 0.5; // cost multiplier

        public double GetShieldMultiplier(string weaponType)
        {
            return weaponType switch
            {
                "Beam" => 1.2,
                "Kinetic" => 0.8,
                "Missile" => 1.0,
                "Pulse" => 1.0,
                _ => 1.0,
            };
        }

        public double GetArmorMultiplier(string weaponType)
        {
            return weaponType switch
            {
                "Beam" => 0.8,
                "Kinetic" => 1.3,
                "Missile" => 1.0,
                "Pulse" => 1.0,
                _ => 1.0,
            };
        }

        public double RangeModifierFor(CombatAction action, ShipState actor, ShipState target)
        {
            // very simplified mapping
            return action == CombatAction.ManeuverToFar ? 0.7 : 1.0;
        }
    }

    public class ActionResult
    {
        public List<string> Messages { get; set; }
        public int ThreatDelta { get; set; }
    }
}
