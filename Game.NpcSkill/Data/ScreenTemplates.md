# Screen Templates

Named screen templates that NPCs or mechanics can reference. Use placeholders consistent with GameMechanics.md.

## Template: opening
AUGMENTED INTERFACE v3.7
HANDLE: {player.name}
CYCLE: {cycle}
LOC: {player.location}

🛡 {player.stats.shields_pct}%  ⚡️ {player.stats.power_pct}%  📡 {comms_status}  🧠 {mind_status}
⚠️ {alert}

SCENE:
{scene_text}

CHOOSE:
{choices}

---

## Template: dialogue
**{npc.name}** — *{npc.title}*

{body}

---
Suggested choices:
{choices}

## Template: action_result
**Action Result — {npc.name}**

{body}

Status: {player.location} | Fuel: {player.fuel} | HP: {player.stats.hp}

{choices}

## Template: mission_offer
**{npc.name}** — *{npc.title}*

Mission Offer: **{mission.title}**

{mission.summary}

Reward: {mission.reward}

Choices:
{choices}

## Template: combat_offer
**{npc.name}** — *{npc.title}**

Enemy Encounter!

{body}

Choices:
1) Fight
2) Flee
3) Attempt Negotiate

## Template: travel_result
**Arrival — {player.name}**

You have arrived at {player.location}.

{body}

{choices}

## Template: combat_contact
⚔️ CONTACT: {enemy.name}
RANGE: {range}

🛡 {player.stats.shields_pct}%  ⚡️ {player.stats.power_pct}%  📡 {comms_status}  🧠 {mind_status}
THREAT CLOCK: {threat_clock} ({threat_progress}/{threat_total})

CHOOSE:
{choices}
