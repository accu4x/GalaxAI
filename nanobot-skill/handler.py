"""GalaxAI Nanobot Telegram handler (polling mode for dev)

Requirements: python-telegram-bot, httpx, pyyaml
"""
import os
import yaml
import httpx
from telegram import InlineKeyboardButton, InlineKeyboardMarkup, Update
from telegram.ext import Updater, CommandHandler, CallbackQueryHandler, CallbackContext

CONFIG_PATH = os.path.join(os.path.dirname(__file__), '..', 'config.yaml')
with open(CONFIG_PATH) as f:
    cfg = yaml.safe_load(f)

GAME_API = cfg.get('game_api', {}).get('base_url', 'http://localhost:5000')
AUTH_HEADER = cfg.get('game_api', {}).get('auth_header', 'X-GALAXAI-KEY')

TELEGRAM_TOKEN = os.environ.get('TELEGRAM_TOKEN')

client = httpx.Client()

def start(update: Update, context: CallbackContext):
    update.message.reply_text('Welcome to GalaxAI. Use /status to check your ship.')

def status(update: Update, context: CallbackContext):
    player_id = str(update.message.from_user.id)
    r = client.get(f"{GAME_API}/player/{player_id}/state")
    if r.status_code == 200:
        data = r.json()
        text = f"Location: {data.get('location')}\nHull: {data.get('hull')}"
        update.message.reply_text(text)
    else:
        update.message.reply_text('Could not fetch state; is the Game API running?')

def main():
    updater = Updater(TELEGRAM_TOKEN)
    dp = updater.dispatcher
    dp.add_handler(CommandHandler('start', start))
    dp.add_handler(CommandHandler('status', status))
    updater.start_polling()
    updater.idle()

if __name__ == '__main__':
    main()
