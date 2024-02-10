using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
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

        private async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            Message message = update.Message;
            if (!string.IsNullOrEmpty(message.Text))
            {
                switch (message.Text)
                {
                    case "/start":
                        await client.SendTextMessageAsync(message.Chat.Id, "пупурлупа!");
                        break;
                    default:
                        ProgramState.ShowInfoDialog(message.Text, "Текст пользователя");
                        break;
                }
            }
        }
        public static void SendMessage(ChatId cid, string text)
        {
            client.SendTextMessageAsync(cid, text);
        }
        public static void SendMessage(ChatId cid, InputFile doc)
        {
            client.SendDocumentAsync(cid, doc);
        }
    }
}
