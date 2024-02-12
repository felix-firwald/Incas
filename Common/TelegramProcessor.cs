using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Windows.Media.SpeechSynthesis;

namespace Incubator_2.Common
{
    public static class TelegramProcessor
    {
        private static TelegramBotClient client;

        public static void StartBot(string token)
        {
            client = new(token);
            client.StartReceiving(Update, Error);
            
        }

        private async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            
        }

        private async static Task Update(ITelegramBotClient cli, Update update, CancellationToken token)
        {
            try
            {
                Message message = update.Message;
                InlineQuery inline = update.InlineQuery;
                switch (update.Type)
                {
                    case UpdateType.Message:
                        if (!string.IsNullOrEmpty(message.Text))
                        {
                            switch (message.Text)
                            {
                                case "/start":
                                    await SendStartMessage(message.Chat.Id);
                                    break;
                                default:
                                    ProgramState.ShowInfoDialog(message.Text, "Текст пользователя");
                                    break;
                            }
                        }
                        break;
                    case UpdateType.CallbackQuery:
                        SwitchOnCallbackQuery(update.CallbackQuery.Data);
                        break;
                }
            }
            catch { }
        }
        private static void SwitchOnCallbackQuery(string callback)
        {
            //InlineKeyboardMarkup inlineKeyboard = new(InlineKeyboardButton.WithCallbackData(text: "Создать документ", callbackData: "NEWDOC"));

        }
        private static Task<Message> SendStartMessage(ChatId cid)
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                [
                    InlineKeyboardButton.WithCallbackData(text: "Создать документ", callbackData: "NEWDOC"),
                    InlineKeyboardButton.WithCallbackData(text: "Получить созданный документ", callbackData: "GETDOC"),
                ],
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Получить созданный документ", callbackData: "GETDOC"),
                },
            });
            return client.SendTextMessageAsync(
                cid,
                "Привет!\nВыбери одну из команд ниже:",
                replyMarkup: inlineKeyboard
                );
        }
        public static Task<Message> SendMessage(ChatId cid, string text)
        {
            return client.SendTextMessageAsync(cid, text);
        }
        public static void SendMessage(ChatId cid, InputFile doc)
        {
            client.SendDocumentAsync(cid, doc);
        }
    }
}
