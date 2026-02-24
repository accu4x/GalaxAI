GalaxAI Nanobot Telegram Skill (scaffold)

Purpose: Telegram adapter that receives user updates, calls Game API endpoints (GET state / POST action), renders messages and inline keyboards.

Stack: Python 3.x, python-telegram-bot, requests/httpx

Files to implement:
- handler.py — receives updates and routes them
- client.py — thin client for Game API (get_state, post_action)
- lore_loader.py — loads markdown files from lore/ for prompts
- config.yaml — reads Game API base_url and TELEGRAM_TOKEN env var

Run locally:
- pip install python-telegram-bot httpx pyyaml
- export TELEGRAM_TOKEN=...
- python handler.py (polling mode for dev) or configure webhook for production
