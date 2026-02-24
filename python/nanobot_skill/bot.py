import os
import logging
import requests
from telegram import Update, InlineKeyboardButton, InlineKeyboardMarkup
from telegram.ext import ApplicationBuilder, CommandHandler, CallbackQueryHandler, ContextTypes

API_URL = os.environ.get('GALAXAI_API_URL','http://localhost:5000')
TOKEN = os.environ.get('TELEGRAM_TOKEN')

logging.basicConfig(level=logging.INFO)

async def start(update: Update, context: ContextTypes.DEFAULT_TYPE):
    await update.message.reply_text('Welcome to GalaxAI. Use /me to get your state.')

async def me(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_id = str(update.effective_user.id)
    resp = requests.get(f"{API_URL}/api/player/{user_id}/state")
    if resp.status_code == 200:
        data = resp.json()
        text = f"Location: {data.get('location')}\nHP: {data.get('hp')}\nInventory: {data.get('inventory')}"
        buttons = [[InlineKeyboardButton('Wait', callback_data='action:wait')]]
        await update.message.reply_text(text, reply_markup=InlineKeyboardMarkup(buttons))
    else:
        await update.message.reply_text('No state found. Try creating an account via the web UI (not yet implemented).')

async def button_handler(update: Update, context: ContextTypes.DEFAULT_TYPE):
    query = update.callback_query
    await query.answer()
    data = query.data
    if data.startswith('action:'):
        action = data.split(':',1)[1]
        user_id = str(query.from_user.id)
        resp = requests.post(f"{API_URL}/api/player/{user_id}/action", json={"actionId":action})
        if resp.status_code == 200:
            r = resp.json()
            await query.edit_message_text(r.get('message'))
        else:
            await query.edit_message_text('Action failed')

if __name__ == '__main__':
    app = ApplicationBuilder().token(TOKEN).build()
    app.add_handler(CommandHandler('start', start))
    app.add_handler(CommandHandler('me', me))
    app.add_handler(CallbackQueryHandler(button_handler))
    app.run_polling()
