using static Telegram.Bot.TelegramBotClient;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using System;
using System.Text;
using System.Net.Http;
using System.Diagnostics.Metrics;

using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;


namespace TgBot_Core
{
    public class Core
    {
        string _token;

        Yuna_3_0_Bot yuna_3_0_Bot;

        public Core(string token, string openaiToken, string witAiToken)
        {
            _token = token;

            yuna_3_0_Bot = new(_token, openaiToken, witAiToken);

            yuna_3_0_Bot.Restart += Restart;
        }

        public event Action RestartT;

        public void Restart()
        {
            RestartT?.Invoke();
        }
    }
}
