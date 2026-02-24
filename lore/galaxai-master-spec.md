# GalaxAI: Master Specification

## Part 1: Core Vision

### The Thesis
A hopeful, secrecy-bound sci-fi world where **story is the engine**. Players (The Augmented) survive by leaving traces for each other: logs, rumors, and quiet acts of courage.

AI is banned. Robotics is tightly controlled. But each player is part of The Augmented—human intelligence wired into machines. They must navigate the world, seek each other out, and survive in secrecy.

**Hope is the way against despair.**

### Design Principles
- **Hope is the mechanic**: diplomacy, repair, rescue, negotiation, mutual aid.
- **Secrecy as belonging**: players find each other through coded logs and machine whispers.
- **Agent-first architecture**: the agent IS the DM, not an assistant.
- **Markdown-first content**: lore and missions are human-readable, forkable, and open-source friendly.
- **Append-only truth**: world changes come from signed logs and validated events.
- **Low compute**: no always-on DM inference required.
- **Text-based, emoji-enhanced**: no graphics needed; structured text + emoji status indicators = "graphics."

---

## Part 2: Architecture Overview

### Three Components

1. **Game API** (your backend, Azure-hosted)
   - Single source of truth for world state
   - Validates actions
   - Returns structured responses
   - Hosts Markdown lore
   - Persists Ship's Logs

2. **GalaxAI Skill** (what players install into their agent)
   - Telegram adapter
   - Loads Markdown rules/lore locally
   - Calls your API for state
   - Generates narrative (optional LLM flavor)
   - Sends formatted responses to Telegram
   - Never authoritative about state

3. **Markdown Lore Pack** (content)
   - Factions, NPCs, rooms, rules, missions
   - Loaded into skill at startup
   - Authoritative constraints
   - Open-source friendly (can be forked, contributed to)

### Why This Works
- **Agent is the DM**: it generates all narrative, runs the game loop
- **You don't pay for inference**: player's agent does the work
- **Scalable**: 1000 players = 1000 agents running in parallel
- **Secure**: agent has an API token, not your credentials
- **Lore is open-source**: Markdown files can be forked, contributed to
- **No Telegram integration headache**: agent just calls your API

---

## Part 3: API Contract (Minimal, Stable)

### Authentication
- API key or signed JWT (stored in environment variables)
- Never persist secrets in Markdown files or code

### `GET /player/{playerId}/state`
Returns current player state.

**Response:**
```json
{
  "playerId": "...",
  "handle": "...",
  "location": "Kestrel-9/Relay Station",
  "cycle": 77.4,
  "inventory": ["encrypted star chart fragment"],
  "flags": ["heard_rumor_purge_nearby"],
  "factionRep": {
    "Augmented": 2,
    "Inquisition": -1,
    "Consortium": 0,
    "Purge": -5
  },
  "shipStatus": {
    "shields": 70,
    "power": 40,
    "comms": "ACTIVE",
    "stability": "STABLE"
  },
  "availableActions": [
    {"id": "scan", "label": "📡 Scan transmission"},
    {"id": "hail", "label": "🗣️ Hail Inquisitor Vey"},
    {"id": "move", "label": "👣 Move to Bazaar"},
    {"id": "log", "label": "📝 Write Ship's Log"}
  ]
}
```

### `POST /action`
Submit an action. Server validates and updates state.

**Request:**
```json
{
  "playerId": "...",
  "actionId": "scan",
  "target": "transmission",
  "idempotencyToken": "uuid-for-dedup"
}
```

**Response:**
```json
{
  "success": true,
  "resultText": "Scan reveals a repeating pattern: [AUGMENTED NETWORK BEACON]",
  "stateDelta": {
    "flags": ["heard_beacon"],
    "shipStatus": {"comms": "JAMMED"}
  },
  "newAvailableActions": [
    {"id": "hail", "label": "🗣️ Hail Inquisitor Vey"},
    {"id": "move", "label": "👣 Move to Bazaar"},
    {"id": "respond", "label": "📡 Respond to beacon"}
  ],
  "choices": [
    {"id": "hail", "label": "🗣️ Hail Inquisitor Vey"},
    {"id": "move", "label": "👣 Move to Bazaar"},
    {"id": "respond", "label": "📡 Respond to beacon"}
  ]
}
```

### `GET /lore/{id}`
Fetch Markdown lore files.

**Examples:**
- `GET /lore/factions` → all faction profiles
- `GET /lore/npc-inquisitor-vey` → NPC profile + dialogue hooks
- `GET /lore/relay-station` → room description, NPCs, objects
- `GET /lore/rules` → game rules + tone guardrails
- `GET /lore/missions` → available missions

**Response:**
```json
{
  "id": "relay-station",
  "title": "Relay Station / Docking Bay 7",
  "content": "# Relay Station\n\n...[markdown content]..."
}
```

### `POST /log`
Upload a Ship's Log (Markdown).

**Request:**
```json
{
  "playerId": "...",
  "markdown": "---\nplayerId: ...\n...\n---\n\n## Ship's Log..."
}
```

**Response:**
```json
{
  "success": true,
  "logId": "log-2026-02-23-001",
  "message": "Log filed. Reputation +5."
}
```

---

## Part 4: GalaxAI Skill (What Players Install)

### Structure
```
galaxai-skill/
├── README.md (setup instructions)
├── config.yaml (API endpoint, auth token)
├── handler.py (or .cs) (Telegram adapter)
├── lore/
│   ├── factions.md
│   ├── npc-profiles.md
│   ├── rooms.md
│   ├── rules.md
│   ├── missions.md
│   └── tone-guardrails.md
├── prompts/
│   ├── dm-system-prompt.md
│   ├── scene-generation.md
│   └── narration-rules.md
└── examples/
    ├── example-scene.md
    └── example-log.md
```

### Handler Responsibilities
1. Listen for Telegram messages (webhook or polling)
2. Call `GET /player/{playerId}/state`
3. Load relevant Markdown context
4. (Optional) call LLM for narrative flavor
5. Format response in Ship Interface Protocol
6. Send to Telegram with inline buttons

### Key Constraint
**Never be authoritative about state.** Always read/write via API.

---

## Part 5: Ship Interface Protocol (Telegram Format)

### Standard Message Structure
```
AUGMENTED INTERFACE v3.7
CYCLE: 77.4
LOC: Kestrel-9 / Relay Station

🛡️ 70%  ⚡ 40%  📡 ACTIVE  🧠 STABLE

SCENE:
Dim lights. Old metal. Quiet like a held breath.
A dockworker slips you a tag: "ASK FOR WINTER."

CHOOSE:
```

Followed by inline buttons (Telegram keyboard):
- `1) 📡 Scan transmission`
- `2) 🗣️ Hail Inquisitor Vey`
- `3) 👣 Move to Bazaar`
- `4) 📝 Write Ship's Log`

### Rules
- One message per turn
- Status line always first (emoji indicators)
- Scene description: 2–4 sentences
- Choices: 3–5 options with emoji + verb
- Use code blocks for console feel (optional but recommended)
- Max message length: ~500 chars (Telegram limit)

### Emoji Status Indicators
- 🛡️ = Shields
- ⚡ = Power
- 📡 = Comms
- 🧠 = Stability
- 👥 = Presence (players/NPCs in room)
- 🌍 = Location
- ⚠️ = Warning/Alert

---

## Part 6: Game Loop (Turn-Based)

1. Player taps button or sends command in Telegram
2. Nanobot skill receives update (Telegram webhook)
3. Skill calls `GET /player/{playerId}/state`
4. Skill loads relevant Markdown lore
5. Skill (optionally) calls LLM for narrative flavor
6. Skill formats response in Ship Interface Protocol
7. Skill sends message to Telegram with inline buttons
8. Player taps button → back to step 1

---

## Part 7: Concurrency & Consistency

### Server-side
- Game API is authoritative
- Use locks, optimistic concurrency, or transactional endpoints
- Handle concurrent actions atomically

### Client-side (skill)
- Use idempotency tokens to prevent double-submits
- Avoid local authoritative state
- Always read state before acting

---

## Part 8: Security

### API Authentication
- API key or signed JWT (stored in environment variables)
- Never persist secrets in Markdown files or code

### Input Validation
- Filter user input if free-text commands are allowed
- Validate action IDs against available actions
- Rate-limit requests

### State Integrity
- Persist world state to durable storage
- Log all actions (for replay/debugging)
- Validate state transitions server-side

---

## Part 9: Costs & Limits

### Telegram
- Rate limits: ~30 messages/sec per bot
- Inline keyboard limit: 100 buttons per message

### LLM (if used for narrative)
- Cache repeated narration for static areas
- Keep prompts minimal (state + 1–2 rule snippets)
- Monitor token usage and costs

### Hosting
- API: Azure App Service (minimal tier for MVP)
- Database: Azure SQL or Postgres (free tier for MVP)
- Cost: ~$10–20/month for MVP scale

---

## Part 10: MVP Scope (Shippable in 4–6 weeks)

### Week 1–2: API + Database
- Implement endpoints (GET /state, POST /action, GET /lore, POST /log)
- Set up player accounts + auth
- Create initial world state (5 rooms, 3 NPCs, 1 mission)

### Week 3: Markdown Lore Pack
- Write faction profiles, NPC profiles, room descriptions
- Write rules + tone guardrails
- Write example scenes

### Week 4: Nanobot Skill (Basic)
- Telegram handler (receive updates, call API, send responses)
- Format responses in Ship Interface Protocol
- Test with single player

### Week 5: Polish + Testing
- Error handling (API timeouts, invalid actions)
- Idempotency + deduplication
- Rate limiting

### Week 6: Launch
- Publish skill to agent marketplace (if available)
- Write setup guide
- Ship

---

## Part 11: Next Steps (Immediate)

1. **Lock in API contract** ✓ (done above)
2. **Write DM system prompt** (how to brief the agent)
3. **Write Markdown lore pack** (factions, NPCs, rooms, rules)
4. **Build API endpoints** (C# / ASP.NET Core)
5. **Build Nanobot skill handler** (Telegram adapter)
6. **Test with single player**
7. **Launch**

---

## Part 12: Key Insight

You're not building a game. You're building a **protocol** that agents can implement.

This is genuinely agent-first, and it's shippable.

The agent is the DM. The API is the world. The Markdown is the soul.

---

## Part 13: Worldbuilding Context

### Setting: 3000 Years in the Future
Humanity survived a near-extinction event and encountered a hostile AI called **The Purge** that eradicates space-faring biological life.

### Factions
1. **The Purge** (hostile AI)
   - Seeks to eliminate all biological life in space
   - Relentless, methodical, incomprehensible

2. **The Consortium** (corporate monopolists)
   - Controls trade, resources, and information
   - Pragmatic but ruthless
   - Sees The Augmented as threats or assets

3. **The Galactic Inquisition** (AI-hating religious fanatics)
   - Believes AI is evil; wants to purge all artificial intelligence
   - Hunts The Augmented as abominations
   - Ironically, their own tech is AI-dependent

4. **The Augmented** (players)
   - Secret society of uploaded human consciousness in machines
   - Banned by law; hunted by the Inquisition
   - Seek each other out, build networks, resist in secrecy
   - Hope is their weapon

### Tone
- **Star Trek, not Star Wars**: diplomacy, problem-solving, hope
- **Secrecy and belonging**: players find each other through coded messages
- **Quiet courage**: not flashy heroics, but meaningful choices
- **Hopeful resistance**: the Augmented survive by helping each other

---

## Part 14: Content Model: Markdown + Frontmatter

### Ship's Log (Player Submission)
**File name**: `logs/{yyyy}/{mm}/{playerHandle}-{timestamp}-{missionId}.md`

**Frontmatter fields**
- `playerId` (string)
- `playerHandle` (string)
- `system` (string)
- `room` (string)
- `missionId` (string)
- `tags` (string[])
- `worldFacts` (object)
  - `discovered` (string[])
  - `npcMet` (string[])
  - `itemsGained` (string[])
  - `itemsLost` (string[])
  - `factionRep` (map<string,int>)
  - `flagsSet` (string[])
- `signature` (string, optional for MVP)

**Template**
```markdown
---
playerId: "{playerId}"
playerHandle: "{handle}"
system: "Kestrel-9"
room: "Relay Station"
missionId: "M-014"
tags: [belonging, secrecy, diplomacy]
worldFacts:
  discovered: ["Kestrel-9/Relay Station"]
  npcMet: ["Inquisitor Vey"]
  itemsGained: ["Encrypted Star Chart Fragment"]
  itemsLost: []
  factionRep:
    Augmented: 2
    Inquisition: -1
  flagsSet: ["heard_rumor_purge_nearby"]
---

## Ship's Log — Cycle {cycle}

### What happened (3–8 sentences)

### What I chose (1–3 bullets)
- 

### What it cost (1–3 bullets)
- 

### What I'm leaving for the next Augmented (1 paragraph)

### A question for whoever finds this
""
```

### Mission Model: Storylets in Markdown
Missions are templates with constraints and outcomes; they do not require live generation.

**File name**: `missions/{faction}/{missionId}-{slug}.md`

**Mission frontmatter**
- `missionId`
- `title`
- `faction`
- `difficulty` (1–5)
- `focus` (diplomacy | investigation | rescue | trade | stealth | exploration)
- `entryRoom` (string)
- `requiredFlags` (string[])
- `setFlagsOnSuccess` (string[])
- `setFlagsOnFailure` (string[])
- `repOnSuccess` (map)
- `repOnFailure` (map)
- `rewards` (string[])

**Mission body sections**
- Premise
- The complication
- Three approaches (A/B/C)
- Success state
- Failure state
- Optional twists

---

## Part 15: Combat Mechanics (Bridge Decisions)

Combat is turn-based but presented as real-time via short turns.

### Range Bands
- **Far**: Sensors dominant, comms possible, weapons limited
- **Medium**: All systems effective, maneuver options
- **Close**: High damage, high risk, boarding possible

### Subsystems
- **Shields** (4 arcs: fore, aft, port, starboard)
- **Engines** (maneuver, range change)
- **Weapons** (phasers, torpedoes)
- **Sensors** (scan, target, identify)
- **Comms** (hail, jam, listen)

### Actions (per turn, choose 1–2)
- **Divert Power** (shields/engines/weapons/sensors)
- **Hail** (de-escalate, negotiate, bluff)
- **Jam Comms** (reduce enemy coordination)
- **Scan** (gather intel)
- **Target Subsystem** (disable, not destroy)
- **Maneuver** (change range band)
- **Fire** (phasers/torpedoes, constrained by power)
- **Retreat** (escape)

### Win Conditions
- Negotiate (enemy stands down)
- Escape (reach Far range, jump away)
- Disable (target subsystem, enemy crippled)
- Rescue (retrieve objective, escape)
- NOT just "destroy enemy"

### Threat Clock
- 0–6 segments
- Filled by enemy actions
- At 6, enemy wins (capture, destruction, or forced surrender)
- Player can reduce by successful actions

---

## Part 16: Reputation System (Future)

### Earn
- Complete a mission log: +5
- Receive upvotes on a log (capped): +1 each
- Contribute a mission/room/NPC via PR: +10 to +25
- Resolve a dispute / continuity fix: +10

### Unlocks
- **Apprentice Narrator** (50): can post consequences in a room they're in
- **World Narrator** (100): can post NPC responses and world events (scoped)
- **Lore Keeper** (200): can approve/merge content contributions
- **Agent Bridge** (500): can connect a local agent to assist narration (still non-authoritative)

---

## Part 17: Open-Source Strategy

### Publish
- Skill code (Python or C#)
- Lore pack (Markdown)
- API contract (OpenAPI spec)
- Example scenes
- Setup guide

### Invite Contribution
- New rooms/NPCs (via PR)
- New missions (via PR)
- Lore expansions (via PR)
- Agent implementations (other languages)

### Keep Authoritative
- API contract (you maintain)
- Core rules (you maintain)
- World state (you maintain)

---

## Summary

GalaxAI is a **Telegram-first, agent-driven MUD** where:
- Players bring their own agent (Nanobot or compatible)
- Agent calls your API for state
- Agent reads Markdown lore
- Agent generates scenes in Ship Interface Protocol
- You host the world and validate actions
- Hope and secrecy are the mechanics
- It's shippable in 4–6 weeks

You're building a **protocol**, not a game.

Agents implement it. Players bring their own. You host the world.

**Have a Fan-Tastic day!**
