GalaxAI

A Telegram-first, agent-driven sci-fi MUD where your agent is the Dungeon Master.

GalaxAI is a protocol + reference implementation:

- The API is the world (authoritative state + validation)
- Markdown is the soul (rules, lore, missions)
- The agent is the DM (narration + UI in Telegram)

If you can make HTTP GET/POST calls and read Markdown, you can run GalaxAI.

Why GalaxAI

Most AI storytelling games centralize inference in one expensive DM bot. GalaxAI flips it:

- Players bring their own agent (Nanobot or compatible)
- The agent runs the game loop in Telegram
- The server only stores state and enforces rules

This makes the game:

- Cheap to host
- Scalable
- Moddable (Markdown content packs)
- Agent-first by design

What's in this repo

- api/ — Authoritative game state API (reference implementation)
- skill/ — Example agent “DM skill” (Telegram adapter + prompts)
- lore/ — Markdown lore pack (rules, factions, NPCs, rooms, missions)
- spec/ — Protocol + API contract (OpenAPI, message format)
- examples/ — Sample scenes, Ship’s Logs, test payloads

How it works (high level)

- Player chats in Telegram.
- Their agent (DM) receives the update.
- The agent calls the GalaxAI API to fetch state and submit actions.
- The agent consults Markdown rules/lore.
- The agent replies in the Ship Interface Protocol (structured text + emoji + buttons).

Important: The agent is never authoritative about state. The API is.

Ship Interface Protocol (Telegram)

Each turn is a single “console screen” + inline buttons.

AUGMENTED INTERFACE v3.7
CYCLE: 77.4
LOC: Kestrel-9 / Relay Station

🛡 70%  ⚡️ 40%  📡 ACTIVE  🧠 STABLE

SCENE:
Dim lights. Old metal. Quiet like a held breath.
A dockworker slips you a tag: "ASK FOR WINTER."

CHOOSE:

Buttons map to actionIds returned by the API.

API (minimal contract)

Core endpoints:
- GET /player/{playerId}/state
- POST /action
- GET /lore/{id}
- POST /log

See spec/openapi.yaml for the full contract.

Quickstart (local dev)

1) Run the API

cd api
# TODO: add docker-compose or dotnet run steps

2) Configure your agent skill

Copy the example config and set environment variables:

- GALAXAI_API_BASE_URL
- GALAXAI_API_TOKEN
- TELEGRAM_BOT_TOKEN

cd skill
cp config.example.yaml config.yaml
# TODO: run instructions for your agent runtime

3) Start a session

In Telegram, message your bot:

/start

then choose actions via buttons

Content packs (Markdown-first)

All world content lives in Markdown and can be forked.

Key files:
- lore/rules.md — authoritative constraints
- lore/factions.md
- lore/npcs/
- lore/rooms/
- lore/missions/

Ship's Logs

Players can file Ship’s Logs (Markdown) that the API stores and indexes.

Contributing

We welcome:
- New rooms, NPCs, missions (Markdown)
- Bug fixes to the API
- New agent implementations (other languages/runtimes)

Contribution rules
- The API is the source of truth.
- Lore must stay consistent with the tone: hopeful, quiet resistance.
- Avoid power creep: prefer diplomacy, trade, repair, rescue over combat.

Roadmap
- v0.1: Single-player private Telegram sessions
- v0.2: Shared rooms + social discovery via logs
- v0.3: Reputation + narrator unlocks
- v0.4: Content pack registry + moderation workflow

License
TBD (MIT/Apache-2.0 recommended for code; CC-BY for lore).

Credits
Created by the GalaxAI community.

FAQ
Is this a text game?
Yes—by design. Emoji + structured console output are the “graphics.”

Do I need an LLM?
Not strictly. The agent can be deterministic; LLM narration is optional.

Can my agent cheat?
The API validates actions and is authoritative about state.
