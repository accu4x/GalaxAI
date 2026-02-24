# Asteroids and Mining

This document describes asteroid types, size classes, composition, and mining mechanics used in GalaxAI.

## Asteroid Types

C-type (Carbonaceous)
- Composition: organic carbon compounds, clays, silicates, water ice, primitive organics
- Typical yields: organics_raw, silicate_dust, water_ice, carbon_compound
- Appearance: dark, low albedo
- Rarity: common (~75% of field)

S-type (Silicaceous)
- Composition: silicate minerals, olivine, nickel-iron mix
- Typical yields: silicate_ore, nickel_iron_ore, mineral_fragments
- Appearance: brighter, rockier
- Rarity: common-to-uncommon (~20% of field)

M-type (Metallic)
- Composition: iron-nickel, platinum-group traces, dense metal cores
- Typical yields: iron_ore, nickel_ore, pgm_nugget (rare)
- Appearance: metallic, high density
- Rarity: uncommon-to-rare (~5% of field)

## Size Classes
- Small: resource pool 50–200 units
- Medium: resource pool 200–1000 units
- Large: resource pool 1000–5000 units

Extraction unit: an abstract unit that corresponds to a single mining "yield" calculation. Modules and ship bonuses scale the units returned per action.

## Mining Modules
- Basic Miner (shuttle): min 1 / max 3 units per action; efficiency 0.8; low energy
- Laser Miner (frigate / industrial): min 2 / max 6 units; efficiency 1.0; medium energy
- Extractor Rig (industrial): min 10 / max 60 units per batch; efficiency 1.6; high setup time

## Yield Mechanics
- When a mining action occurs, the server calculates base_units = randInt(minYield, maxYield) * efficiencyModifiers
- For each unit, a material is sampled using the asteroid's composition weights
- Rare byproducts (pgm_nugget, exotic_organics) have low chance (1–5%) depending on asteroid type and module efficiency
- Asteroid.pool -= base_units; when pool <= 0 the asteroid is exhausted

## Refining
- Refining converts raw materials to higher-value refined goods using facility or onboard refinery module
- Example recipes:
  - 10 iron_ore -> 1 iron_ingot
  - 5 water_ice -> 1 processed_fuel
- Refining consumes time and energy and may require blueprints or research modifiers

## Relic Hunting
- Relics are rare items left by the Archons (ancient builders). They spawn on planets, moons, and ruin-sites.
- SEARCH_RELIC action:
  - Preconditions: at a location with relic potential; specialized scanners increase find chance
  - Effects: small chance to produce relic item; finding a relic may increase faction attention or Purge detection
- RESEARCH_RELIC action:
  - Use research facilities to decode relics and unlock blueprints or story branches. Consumes time and materials.

## Example Mining API action
POST /action
{
  "actionId": "MINE_ASTEROID",
  "playerId": "player-123",
  "params": { "asteroidId": "nova-belt-1/asteroid-7", "moduleId": "laser_mk2" }
}

Response (example):
{
  "event": "ASTEROID_MINED",
  "result": {
    "yield": [ { "itemId": "ore_m_iron", "qty": 5 }, { "itemId": "pgm_nugget", "qty": 1 } ],
    "asteroidRemaining": 345,
    "eventsTriggered": ["PIRATE_INTEREST"]
  }
}
