using Common;
using Models;
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

        private async static System.Threading.Tasks.Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            
        }

        private async static System.Threading.Tasks.Task Update(ITelegramBotClient cli, Update update, CancellationToken token)
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
                                    if (message.Text.StartsWith("Category#"))
                                    {
                                        ShowTemplates(message.Chat.Id, message.Text.Replace("Category#", ""));
                                    }
                                    break;
                            }
                        }
                        break;
                    case UpdateType.CallbackQuery:
                        SwitchOnCallbackQuery(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Data);
                        break;
                }
            }
            catch { }
        }
        private static void ShowTemplates(long chat, string category)
        {
            string result = "Выберите шаблон:";
            using (Template t = new())
            {
                foreach (STemplate item in t.GetWordTemplates(category))
                {
                    result += $"\n`{item.name}`\n";
                }
            }
            client.SendTextMessageAsync(chatId: chat, text: result, parseMode: ParseMode.MarkdownV2);
        }
        private static void SwitchOnCallbackQuery(long chat, string callback)
        {
            string message = "";
            switch (callback)
            {
                case "NEWDOC":
                    using (Template t = new())
                    {
                        List<string> categories = t.GetCategories();
                        string result = "Выберите *одну* из категорий ниже:";
                        foreach (string category in categories)
                        {
                            if (string.IsNullOrWhiteSpace(category)) continue;
                            result += $"\n`Category#{category}`\n";
                        }
                        message = result;
                    }
                    break;
                case "GETDOC":
                    break;
                case "SELECTUSER":
                    break;
            }
            client.SendTextMessageAsync(chatId: chat, text: message, parseMode: ParseMode.MarkdownV2);
        }
        private static Task<Message> SendStartMessage(ChatId cid)
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                [
                    InlineKeyboardButton.WithCallbackData(text: "Создать документ", callbackData: "NEWDOC"),
                ],
                // second row
                [
                    InlineKeyboardButton.WithCallbackData(text: "Получить созданный документ", callbackData: "GETDOC"),
                ],
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Выбрать пользователя", callbackData: "SELECTUSER"),
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
