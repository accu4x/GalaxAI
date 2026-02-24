GalaxAI nanobot Telegram skill (skeleton)

Setup
-----
1. Create venv: python -m venv .venv
2. pip install -r requirements.txt
3. set TELEGRAM_TOKEN and GALAXAI_API_URL
4. python bot.py

Functionality
-------------
- /me shows player state by GET /api/player/{id}/state
- Inline button for 'Wait' posts action 'wait' to POST /api/player/{id}/action

Notes
-----
This is a minimal skeleton. Extend to support better UI, markdown-based lore prompts, and user-provided LLM agent hooks.
