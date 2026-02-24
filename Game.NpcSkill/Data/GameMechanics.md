# Game Mechanics

This file documents canonical game actions, state transitions, object shapes, and high-level workflows the DialogRunner and other skills can consult.

## Placeholders
Canonical placeholders that screen templates and NPC screens can use:
- {npc.name}, {npc.title}, {npc.portrait}
- {player.name}, {player.level}, {player.location}
- {player.stats.hp}, {player.stats.shields}, {player.fuel}
- {player.stats.shields_pct}, {player.stats.power_pct}
- {mission.id}, {mission.title}, {mission.reward}, {mission.summary}
- {cycle} — current game cycle or time
- {comms_status}, {mind_status}, {alert} — short status strings
- {body} — main reply text
- {choices} — rendered suggested choices list

## Actions

### Newsfeed / AcceptMission
Description: Player checks the local newsfeed and may accept a mission.
- UI: NEWSFEED screen shows list of missions (id, title, short summary, reward)
- AcceptMission preconditions: none (may require skill checks in future)
- AcceptMission effects:
  - add mission to player.missions
  - set mission.status = "assigned"
  - emit event: MISSION_ASSIGNED(missionId)
- Follow-up: suggest "Plot course", "Inspect mission details", "Ignore"

### Undock
Description: Leave dock and enter space.
- Preconditions: ship.docked == true
- Effects: ship.docked = false; player.location = "in-transit"; consume fuel (default 5)
- UI: ACTION_RESULT screen with departure description and current ship status
- Events: SHIP_UNDOCKED

### Travel / JumpTo
Description: Move between locations (station, belt, planet)
- Preconditions: sufficient fuel; navigation system not damaged
- Effects: player.location = destination; consume fuel per distance
- UI: TRAVEL_RESULT screen describing arrival
- Events: ARRIVAL(destination)

### Encounter: Pirates
Description: Random or scripted encounter during transit.
- Trigger: probability on travel to certain zones or mission-specific triggers
- Effects: spawn enemy group; switch to combat subroutine
- UI: COMBAT_OFFER screen (choice to fight, flee, or negotiate)

### Combat (simple resolution)
Description: Quick simulated combat flow used for demos.
- Preconditions: enemy present
- Options: Fight, Flee, AttemptNegotiate
- Resolution (simple): compare player.roll + attack vs enemy.roll + defense; apply damage to player.stats.hp and enemy.hp
- Outcomes: Victory (gain loot/mission progress), Defeat (forced flee or station return), Flee (return to nearest safe station, set ship.docked=false?
- Events: COMBAT_RESOLVED(result)

### Upload Captain's Log
- Description: After mission-critical events the player's ship uploads a log to a station or central authority.
- Effects: mission.progress update, potential reputation change
- UI: LOG_UPLOAD screen

## State transition example: Mining mission (workflow)
1) Player checks NEWSFEED → sees MINING_MISSION_001
2) Player ACCEPT_MISSION(MINING_MISSION_001)
3) Player PLOTS course and UNDOCKs
4) Player TRAVELS to ASTEROID_BELT
5) Encounter: PIRATES appear → COMBAT subroutine
6a) If player flees → return to nearest STATION; UPLOAD captain log; mission marked "failed" or "abandoned"
6b) If player wins → mine asteroids, collect loot, return to station, upload log, complete mission

## Events (canonical)
- MISSION_ASSIGNED(missionId)
- SHIP_UNDOCKED
- ARRIVAL(location)
- ENCOUNTER_PIRATES
- COMBAT_RESOLVED(result)
- LOG_UPLOADED

## Core Loops
- Modular gameplay: the game supports multiple modules (exploration, trading, missions, social hubs). Designers can add modules by creating action definitions and screen templates.
- Technical limitations as fiction: use client limitations as narrative mechanics. For example, new missions or storylines may only be available at specific locations (e.g., read the newsfeed at a station to unlock missions).
- Progressive discovery: when exploring a new star system the server generates a lightweight "system shell". As players explore, the system fleshs out with locations, NPCs, and events. When exploration data is uploaded, other players can discover and visit those sites, allowing the world to evolve collaboratively.

## Mining & Asteroid Mechanics

Overview: Mining is an active gameplay loop where players travel to asteroid belts and extract raw materials. Asteroids are categorized by composition (C-type, S-type, M-type). Each asteroid has a limited resource pool and yields materials according to its composition and the player's mining setup.

Canonical actions and events:
- MINE_ASTEROID(asteroidId, moduleId)
- DEPLOY_EXTRACTOR(asteroidFieldId, extractorModuleId)
- REFINE_MATERIALS(facilityId, recipeId)
- SEARCH_RELIC(locationId)
- RESEARCH_RELIC(relicId, facilityId)

Preconditions & Effects:
- MINE_ASTEROID
  - Preconditions: player.ship at asteroid location; required mining module installed and functional; cargo capacity available
  - Effects: consume capacitor/fuel, reduce asteroid.pool by yieldUnits, add items to ship.cargo or player.inventory, chance to trigger encounters (pirates, Purge drones)
  - Events: ASTEROID_MINED(result)

- DEPLOY_EXTRACTOR
  - Preconditions: large industrial ship or specialized module; requires time to deploy
  - Effects: reserves portion of asteroid pool for extractor; yields large batch harvests over time
  - Events: EXTRACTOR_DEPLOYED, EXTRACTOR_DEPLETED

- REFINE_MATERIALS
  - Preconditions: docked at refinery or have onboard refinery module; requires recipe and input items
  - Effects: consumes inputs, time & energy; produces refined outputs and possibly byproducts
  - Events: REFINERY_COMPLETED

Relic Hunting & Research
- SEARCH_RELIC
  - Description: Rare relics from the Archons can be found on planets, moons, and deep-space ruins. Searching is an action that consumes time and may require specialized scanners or relic-hunting modules.
  - Preconditions: at location with relic potential (ruins, ruins-sites, Arks)
  - Effects: small chance to find a relic item; finding increases local lore data and can trigger faction interest or Purge detection
  - Events: RELIC_FOUND(relicId)

- RESEARCH_RELIC
  - Description: Researching a recovered relic at a lab/refinery can unlock blueprints, new module designs, or story knowledge. Research consumes rare materials, time, and specialist facilities.
  - Preconditions: have relic item, access to research facility
  - Effects: yields blueprint items, research logs, and may adjust faction reputation or trigger hidden story branches
  - Events: RELIC_RESEARCHED(result)

## Items & Data
- Items catalog (JSON): Game.NpcSkill/Data/Items/items.json — includes ore types, refined goods, modules, relics and blueprints
- Mining modules are defined in the items catalog as module-type entries with min/max yields, efficiency multipliers, and energy costs

## Pricing & Markets
- Mining outputs feed system markets. The market tick algorithm uses daily randomness + player-supplied supply/demand deltas. Repeated shipments to a planet increase local supply and depress prices.
- Suggested API endpoints:
  - GET /market/{systemId}/snapshot
  - POST /market/{systemId}/trade { playerId, itemId, qty, buy/sell }
  - GET /player/{playerId}/inventory

## Notes for Designers
- Make extraction deterministic enough to be fun but stochastic enough to support emergent stories and scarcity.
- Use extractor deployment for large-scale industrial play; make it require commitments (time, vulnerability) so it isn't trivially exploitable.
- Relic hunting should be rare and significant — discovering a relic should feel like an event and may yield long-term narrative consequences.

## Notes for Implementation
- All API payloads and state snapshots use JSON. Agents should treat the API as authoritative and only present the screen payloads returned by the server.
- Design mining tests and mock data to exercise the ASTEROID_MINED and RELIC_FOUND events during development.