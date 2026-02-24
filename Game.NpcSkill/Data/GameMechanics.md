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

## Notes for Designers
- Add new actions as simple verbs with Preconditions, Effects, UI, and Events.
- Keep actions idempotent and clearly document required state fields.
- The DialogRunner will consult these rules to decide when to map free-text player intents to game actions.