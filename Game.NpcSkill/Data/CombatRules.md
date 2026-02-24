Combat System — Design Rules

Overview
- Turn-based combat presented as "real-time" in the UI (player sees a combat screen and selects numbered choices each turn).
- Combat uses a 6-step threat clock. Certain actions or delays advance the clock; reaching 6 triggers a severe escalation (reinforcements, Purge arrival, mission failure).
- Damage flow: Shields absorb damage first (if up). Excess spills to armor. Armor is persistent and requires repair. Shields regenerate per tick if capacitor is available.
- Capacitor is consumed to power shields, pulse weapons, and some modules. If capacitor is depleted, shields shut down and some modules go offline.

Core formulas & rules
- Hit chance (per attack):
  hitChance = clamp(baseAccuracy * attackerSkillEffect * rangeModifier * (1 - targetEvasion), 0.05, 0.98)
  - baseAccuracy: weapon stat (0..1)
  - attackerSkillEffect: from skills (0.3..0.8 baseline per tier, plus mastery bonuses)
  - rangeModifier: weapon-specific (close=1.0, medium=0.9, far=0.7)
  - targetEvasion: derived from ship class and piloting skill (0..0.5)

- Damage calculation (on hit):
  rawDamage = weapon.BaseDamage * (1 + randomVariance)
  - Apply damage multipliers by type:
    - Beam: shieldMultiplier = 1.2, armorMultiplier = 0.8
    - Kinetic: shieldMultiplier = 0.8, armorMultiplier = 1.3
    - Missile: shieldMultiplier = 1.0, armorMultiplier = 1.0
    - Pulse: shieldMultiplier = 1.0, armorMultiplier = 1.0 (but high capacitor cost)
  - Shields absorb up to currentShields. If shields >= dmg, shields -= dmg * shieldAbsorbEfficiency; else shields -> 0 and leftover dmg flows to armor.
  - Armor subtracts leftover damage. If armor <= 0, ship is disabled/destroyed.

- Shields recharge per-tick if capacitor >= shieldRechargeThreshold:
  rechargeAmount = min(shieldMax - shieldCurrent, capacitorAvailable * rechargeRate)
  capacitorAvailable -= rechargeEnergyCost

- Threat clock:
  - Starts at 0. Certain failed checks, delaying an action, or using loud actions (missile salvos, extractor rigs) advance the clock by 1 or more steps.
  - At 6, forced escalation (spawn reinforcements, Purge detection, or mission abort depending on encounter).

- Action resolution order per turn:
  1) Player action resolves (attack, hail, maneuver, divert power, target sensors)
  2) Apply immediate effects (capacitor drain, shield changes)
  3) NPCs act
  4) Resolve end-of-turn effects (shield regen, capacitor regen, check threat clock)

Weapon archetype notes
- Beam: good vs shields, medium energy cost, decent accuracy
- Kinetic: best vs armor, long range, uses ammo
- Missiles: balanced vs shields/armor, mid-range, better vs slow ships, uses ammo
- Pulse: short-range, high energy cost, balanced vs both, strong against fast ships (bonus vs high evasion)

Balance notes
- Rock/Paper/Scissors: Frigate vs Cruiser vs Destroyer triangle (configurable in ship profiles).
- Modules and skill tiers modify accuracy, damage, and recharge rates.

Integration notes
- The CombatEngine prototype returns a CombatTurnResult object with: updated ship states, messages to render, choice suggestions, and threat clock delta.
- The API should be authoritative: agents submit actions to POST /action (Melee combat action types) and the server runs the CombatEngine to compute deterministic outcomes.

Tuning
- Many coefficients (base accuracies, damage, recharge rates) are intentionally data-driven and should be stored in a weapon/ship JSON stat table for tuning without code changes.
