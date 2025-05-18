using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TgBot_Core
{
    public class Yuna_3_0_Bot
    {
        ITelegramBotClient _botClient;
        ReceiverOptions _receiverOptions;

        User _bot;

        string _token;

        public Yuna_3_0_Bot(string token)
        {
            _token = token;

            Work();
        }

        async Task Work()
        {
            _botClient = new TelegramBotClient(_token);

            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                    {
                        UpdateType.BusinessConnection,
                        UpdateType.BusinessMessage,
                        UpdateType.CallbackQuery,
                        UpdateType.ChannelPost,
                        UpdateType.ChatBoost,
                        UpdateType.ChatJoinRequest,
                        UpdateType.ChatMember,
                        UpdateType.ChosenInlineResult,
                        UpdateType.DeletedBusinessMessages,
                        UpdateType.EditedBusinessMessage,
                        UpdateType.EditedChannelPost,
                        UpdateType.EditedMessage,
                        UpdateType.InlineQuery,
                        UpdateType.Message,
                        UpdateType.MessageReaction,
                        UpdateType.MessageReactionCount,
                        UpdateType.MyChatMember,
                        UpdateType.Poll,
                        UpdateType.PollAnswer,
                        UpdateType.PreCheckoutQuery,
                        UpdateType.PurchasedPaidMedia,
                        UpdateType.RemovedChatBoost,
                        UpdateType.ShippingQuery,
                        UpdateType.Unknown
                    },
                DropPendingUpdates = true
            };

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            _bot = await _botClient.GetMe();

            Logs_Core.Core.AddLog($"{_bot.FirstName} запущена!", DateTime.Now, (this.GetType()).ToString() + ".Work()", 0);

            await _botClient.SendMessage(1362019388, "Я снова работаю");

            await Task.Delay(-1);
        }

        async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                //Logs_Core.Core.AddLog(update.Type.ToString(), DateTime.Now, (this.GetType()).ToString() + ".UpdateHandler()", 1);

                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            UpdateHandler_Message(update, botClient);

                            break;
                        }

                    case UpdateType.CallbackQuery:
                        {


                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logs_Core.Core.AddLog(ex.ToString(), DateTime.Now, (this.GetType()).ToString() + ".UpdateHandler()", 2);

                Console.WriteLine(ex.ToString());
            }
        }

        async Task UpdateHandler_Message(Update update, ITelegramBotClient botClient)
        {
            try
            {
                Message message = update.Message;
                MessageType messageType = message.Type;
                var user = message.From;
                var chat = message.Chat;



                switch (messageType)
                {
                    case MessageType.Text:
                        {
                            Logs_Core.Core.AddLog(message.Text, DateTime.Now, (this.GetType()).ToString() + ".UpdateHandler_Message()", 1);

                            if(message.Text == "/restart")
                            {
                                Restart?.Invoke();
                            }

                            break;
                        }
                    case MessageType.Voice:
                        {
                            //Logs_Core.Core.AddLog("Voice", DateTime.Now, (this.GetType()).ToString() + ".UpdateHandler_Message()", 1);

                            break;
                        }

                    default:
                        {
                            botClient.SendMessage(chat.Id, "Я вас не поняла(");

                            return;
                        }
                }
            }
            catch
            {

            }
        }

        Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {

                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Logs_Core.Core.AddLog(ErrorMessage, DateTime.Now, (this.GetType()).ToString() + ".ErrorHandler()", 2);

            //Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public event Action Restart;

    }
}
