"""GalaxAI Nanobot Telegram handler (polling mode for dev)

Requirements: python-telegram-bot, httpx, pyyaml
"""
import os
import logging
import yaml
import httpx
from telegram import InlineKeyboardButton, InlineKeyboardMarkup, Update, ParseMode
from telegram.ext import Updater, CommandHandler, CallbackQueryHandler, MessageHandler, Filters, CallbackContext

# load config
CONFIG_PATH = os.path.join(os.path.dirname(__file__), '..', 'config.yaml')
with open(CONFIG_PATH) as f:
    cfg = yaml.safe_load(f)

GAME_API = cfg.get('game_api', {}).get('base_url', 'http://localhost:5000')
AUTH_HEADER = cfg.get('game_api', {}).get('auth_header', 'X-GALAXAI-KEY')

TELEGRAM_TOKEN = os.environ.get('TELEGRAM_TOKEN')

client = httpx.Client(timeout=10.0)
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# in-memory session mapping: chat_id -> session_id
sessions = {}

# helper to send reply and optional inline choices
def send_reply(update: Update, text: str, choices=None):
    # send markdown-formatted text
    if hasattr(update, 'message') and update.message:
        target = update.message.chat_id
    elif hasattr(update, 'callback_query') and update.callback_query:
        target = update.callback_query.message.chat_id
    else:
        target = None

    if choices:
        buttons = [[InlineKeyboardButton(str(i+1), callback_data=str(i)) for i, _ in enumerate(choices)]]
        # also include labelled buttons with full text as callback (safer to send index)
        kb = InlineKeyboardMarkup(buttons)
        if update.callback_query:
            update.callback_query.message.reply_text(text, reply_markup=kb, parse_mode=ParseMode.MARKDOWN)
            update.callback_query.answer()
        else:
            update.message.reply_text(text, reply_markup=kb, parse_mode=ParseMode.MARKDOWN)
    else:
        if update.callback_query:
            update.callback_query.message.reply_text(text, parse_mode=ParseMode.MARKDOWN)
            update.callback_query.answer()
        else:
            update.message.reply_text(text, parse_mode=ParseMode.MARKDOWN)

# /start command
def start(update: Update, context: CallbackContext):
    update.message.reply_text('Welcome to GalaxAI. Use /galaxai <npc_id> to begin a session, or just /galaxai to start.')

# /status - existing helper
def status(update: Update, context: CallbackContext):
    player_id = str(update.message.from_user.id)
    try:
        r = client.get(f"{GAME_API}/player/{player_id}/state")
        if r.status_code == 200:
            data = r.json()
            text = f"Location: {data.get('location')}\nHull: {data.get('hull')}"
            update.message.reply_text(text)
            return
    except Exception as e:
        logger.exception('Failed fetching player state')
    update.message.reply_text('Could not fetch state; is the Game API running?')

# /galaxai command - start a session with an NPC (optional npc_id arg)
def galaxai_start(update: Update, context: CallbackContext):
    chat_id = update.message.chat_id
    args = context.args or []
    npc_id = args[0] if len(args) >= 1 else None
    owner_id = str(update.message.from_user.id)

    payload = { 'ownerId': owner_id }
    if npc_id:
        payload['npcId'] = npc_id

    try:
        r = client.post(f"{GAME_API}/session/start", json=payload)
        if r.status_code == 200:
            j = r.json()
            session_id = j.get('sessionId')
            reply = j.get('reply') or j.get('raw') or '...'
            choices = j.get('choices')
            # store session per chat
            if session_id:
                sessions[str(chat_id)] = session_id
            send_reply(update, reply, choices)
        else:
            update.message.reply_text(f'Could not start session (status {r.status_code}).')
    except Exception as e:
        logger.exception('Error starting session')
        update.message.reply_text('Error contacting Game API to start session.')

# free-text handler - route to active session if present
def message_handler(update: Update, context: CallbackContext):
    chat_id = str(update.message.chat_id)
    if chat_id not in sessions:
        update.message.reply_text('No active GalaxAI session. Use /galaxai to start one.')
        return

    session_id = sessions[chat_id]
    text = update.message.text
    payload = { 'sessionId': session_id, 'message': text }

    try:
        r = client.post(f"{GAME_API}/session/message", json=payload)
        if r.status_code == 200:
            j = r.json()
            reply = j.get('reply') or j.get('raw') or '...'
            choices = j.get('choices')
            send_reply(update, reply, choices)
        else:
            update.message.reply_text(f'Game API returned status {r.status_code} for session message.')
    except Exception:
        logger.exception('Error sending session message')
        update.message.reply_text('Error contacting Game API for session message.')

# callback for inline buttons (choices)
def callback_handler(update: Update, context: CallbackContext):
    query = update.callback_query
    chat_id = str(query.message.chat_id)
    if chat_id not in sessions:
        query.answer('No active session')
        return
    session_id = sessions[chat_id]
    choice_idx = query.data
    # send the chosen option text as the player's message; we don't know the mapping here,
    # so we send the index and let the DialogRunner interpret indexes if it supports that.
    payload = { 'sessionId': session_id, 'message': choice_idx }
    try:
        r = client.post(f"{GAME_API}/session/message", json=payload)
        if r.status_code == 200:
            j = r.json()
            reply = j.get('reply') or j.get('raw') or '...'
            choices = j.get('choices')
            send_reply(update, reply, choices)
        else:
            query.message.reply_text(f'Game API returned status {r.status_code} for choice.')
            query.answer()
    except Exception:
        logger.exception('Error handling callback')
        query.message.reply_text('Error contacting Game API for choice.')
        query.answer()


def quit_session(update: Update, context: CallbackContext):
    chat_id = str(update.message.chat_id)
    if chat_id in sessions:
        del sessions[chat_id]
        update.message.reply_text('Session ended. Use /galaxai to start a new one.')
    else:
        update.message.reply_text('No active session to quit.')


def main():
    if not TELEGRAM_TOKEN:
        logger.error('TELEGRAM_TOKEN not set; exiting')
        return

    updater = Updater(TELEGRAM_TOKEN)
    dp = updater.dispatcher
    dp.add_handler(CommandHandler('start', start))
    dp.add_handler(CommandHandler('status', status))
    dp.add_handler(CommandHandler('galaxai', galaxai_start))
    dp.add_handler(CommandHandler('quit', quit_session))
    dp.add_handler(CallbackQueryHandler(callback_handler))
    # handle plain text messages to forward to session
    dp.add_handler(MessageHandler(Filters.text & ~Filters.command, message_handler))

    updater.start_polling()
    updater.idle()

if __name__ == '__main__':
    main()
