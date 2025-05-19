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
using Editor_Core;
using Voice_Core;

namespace TgBot_Core
{
    public class Yuna_3_0_Bot
    {
        ITelegramBotClient _botClient;
        ReceiverOptions _receiverOptions;

        User _bot;

        string _token;
        string _openaiToken;
        string _witAiToken;

        public Yuna_3_0_Bot(string token, string openaiToken, string witAiToken)
        {
            _token = token;
            _openaiToken = openaiToken;
            _witAiToken = witAiToken;

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

                            else if(message.Text.StartsWith("/commit"))
                            {
                                string commitMessage = message.Text.Substring(7).Trim();
                                string repoPath = @"E:\Yuna_3_0"; // Путь к твоему git-репозиторию

                                if (Git_Core.Core.CommitAndPush(repoPath, commitMessage, out string result))
                                {
                                    await botClient.SendMessage(chat.Id, $"✅ Коммит и пуш выполнены:\n\n{result}");
                                }
                                else
                                {
                                    await botClient.SendMessage(chat.Id, $"❌ Ошибка при пуше:\n\n{result}");
                                }
                            }

                            else if(message.Text.StartsWith("/edit"))
                            {
                                string[] parts = message.Text.Split("          ", 5); // команда + 4 аргумента
                                if (parts.Length < 5)
                                {
                                    await botClient.SendMessage(chat.Id, "❗ Формат команды: /edit <проект> <файл.cs> <паттерн> <замена>");
                                    return;
                                }

                                string project = parts[1];
                                string file = parts[2];
                                string pattern = parts[3];
                                string replacement = parts[4];

                                var editor = new Editor_Core.Core();
                                var builder = new ProjectBuilder();

                                string root = @"E:\Yuna_3_0"; // путь к решению
                                string csprojPath = Path.Combine(root, project, $"{project}.csproj");

                                if (editor.ReplaceInProjectFile(root, project, file, pattern, replacement, out string editMsg))
                                {
                                    if (builder.BuildProject(csprojPath, out string buildMsg))
                                    {
                                        await botClient.SendMessage(chat.Id, $"✅ Код обновлён и собрано:\n{editMsg}");
                                        Restart?.Invoke();
                                    }
                                    else
                                    {
                                        await botClient.SendMessage(chat.Id, $"⚠️ Код обновлён, но сборка не удалась:\n{buildMsg}");
                                    }
                                }
                                else
                                {
                                    await botClient.SendMessage(chat.Id, $"❌ Ошибка редактирования:\n{editMsg}");
                                }
                            }

                            else if(message.Text.StartsWith("/newproject"))
                            {
                                string[] parts = message.Text.Split("          ");
                                if (parts.Length < 2) return;

                                var manager = new SolutionManager(@"E:\Yuna_3_0");
                                if (manager.CreateNewProject(parts[1], out string output))
                                {
                                    await botClient.SendMessage(chat.Id, $"✅ Проект создан:\n{output}");
                                    Restart?.Invoke();
                                }

                                else
                                    await botClient.SendMessage(chat.Id, $"❌ Ошибка:\n{output}");
                            }

                            else if(message.Text.StartsWith("/addref"))
                            {
                                string[] parts = message.Text.Split("          ");
                                if (parts.Length < 3) return;

                                var manager = new SolutionManager(@"E:\Yuna_3_0");
                                if (manager.AddProjectReference(parts[1], parts[2], out string output))
                                {
                                    await botClient.SendMessage(chat.Id, $"✅ Ссылка добавлена:\n{output}");
                                    Restart?.Invoke();
                                }
                                else
                                    await botClient.SendMessage(chat.Id, $"❌ Ошибка:\n{output}");
                            }

                            else if (message.Text.StartsWith("/removeproject"))
                            {
                                string[] parts = message.Text.Split(new[] { "          " }, StringSplitOptions.None);
                                if (parts.Length < 2)
                                {
                                    await botClient.SendMessage(chat.Id, "❗ Формат: /removeproject          <ProjectName>");
                                    return;
                                }

                                var manager = new SolutionManager(@"E:\Yuna_3_0");

                                if (manager.RemoveProject(parts[1], out string output))
                                {
                                    await botClient.SendMessage(chat.Id, $"✅ Проект \"{parts[1]}\" удалён.\n\n{output}");
                                }
                                else
                                {
                                    await botClient.SendMessage(chat.Id, $"❌ Ошибка при удалении проекта:\n\n{output}");
                                }
                            }

                            else if (message.Text.StartsWith("/removeref"))
                            {
                                string[] parts = message.Text.Split(new[] { "          " }, StringSplitOptions.None);
                                if (parts.Length < 3)
                                {
                                    await botClient.SendMessage(chat.Id, "❗ Формат: /removeref          <FromProject>          <ToProject>");
                                    return;
                                }

                                var manager = new SolutionManager(@"E:\Yuna_3_0");

                                if (manager.RemoveProjectReference(parts[1], parts[2], out string output))
                                {
                                    await botClient.SendMessage(chat.Id, $"✅ Ссылка из \"{parts[1]}\" на \"{parts[2]}\" удалена.\n\n{output}");
                                }
                                else
                                {
                                    await botClient.SendMessage(chat.Id, $"❌ Ошибка при удалении ссылки:\n\n{output}");
                                }
                            }
                            else if (message.Text.StartsWith("/ai-edit"))
                            {
                                string[] parts = message.Text.Split(new[] { "          " }, StringSplitOptions.None);
                                if (parts.Length < 4)
                                {
                                    await botClient.SendMessage(chat.Id, "❗ Формат: /ai-edit          <Проект>          <Файл.cs>          <инструкция>");
                                    return;
                                }

                                string project = parts[1];
                                string fileName = parts[2];
                                string instruction = parts[3];

                                string solutionRoot = @"E:\Yuna_3_0";
                                string filePath = Path.Combine(solutionRoot, project, fileName);

                                if (!File.Exists(filePath))
                                {
                                    await botClient.SendMessage(chat.Id, $"❌ Файл не найден: {filePath}");
                                    return;
                                }

                                string originalCode = File.ReadAllText(filePath);

                                var ai = new AI_Core.Core(_openaiToken); // из AI_Core
                                string prompt = $"Ты помощник разработчика. Прими код:\n\n{originalCode}\n\nВнеси изменения по инструкции: {instruction}";
                                string updatedCode = ai.GetUpdatedCode(prompt); // метод ты можешь сам реализовать

                                if (!string.IsNullOrWhiteSpace(updatedCode))
                                {
                                    File.WriteAllText(filePath, updatedCode);
                                    await botClient.SendMessage(chat.Id, $"✅ Изменения внесены с помощью AI и файл обновлён.");
                                }
                                else
                                {
                                    await botClient.SendMessage(chat.Id, $"❌ AI не вернул результат.");
                                }
                            }
                            else if (message.Text.StartsWith("/rollback"))
                            {
                                string repoPath = @"E:\Yuna_3_0";
                                string result = Git_Core.Core.RunGit("reset --hard HEAD~1", repoPath);

                                await botClient.SendMessage(chat.Id, $"↩️ Откат выполнен:\n{result}");
                            }

                            else if (message.Text.StartsWith("/uploadcs"))
                            {
                                string[] parts = message.Text.Split(new[] { "          " }, StringSplitOptions.None);
                                if (parts.Length < 4)
                                {
                                    await botClient.SendMessage(chat.Id, "❗ Формат: /uploadcs          <Проект>          <ИмяФайла.cs>          <Код>");
                                    return;
                                }

                                string project = parts[1];
                                string fileName = parts[2];
                                string rawCode = parts[3];

                                var editor = new Editor_Core.Core();
                                string root = @"E:\Yuna_3_0";

                                if (editor.SaveFileToProject(root, project, fileName, rawCode, out string result))
                                    await botClient.SendMessage(chat.Id, result);
                                else
                                    await botClient.SendMessage(chat.Id, result);
                            }

                            else if (message.Text.StartsWith("/git"))
                            {
                                string[] parts = message.Text.Split(new[] { "          " }, StringSplitOptions.None);
                                if (parts.Length < 2)
                                {
                                    await botClient.SendMessage(chat.Id, "❗️ Формат: /git          <аргументы git>");
                                    return;
                                }

                                string gitArgs = parts[1];
                                string repoPath = @"E:\Yuna_3_0";

                                try
                                {
                                    string result = Git_Core.Core.RunGit(gitArgs, repoPath);
                                    await botClient.SendMessage(chat.Id, $"✅ Результат:{result}");
                                }
                                catch (Exception ex)
                                {
                                    await botClient.SendMessage(chat.Id, $"❌ Ошибка:{ex.Message}");
                                }
                            }          // <<<COMMANDS>>>

                            break;
                        }
                    case MessageType.Voice:
                        {
                            //Logs_Core.Core.AddLog("Voice", DateTime.Now, (this.GetType()).ToString() + ".UpdateHandler_Message()", 1);

                            if (message.Voice != null)
                            {
                                var fileId = message.Voice.FileId;

                                // 1. Получаем файл
                                var file = await botClient.GetFile(fileId);
                                var filePath = file.FilePath;

                                // 2. Скачиваем как byte[]
                                string url = $"https://api.telegram.org/file/bot{_token}/{filePath}";
                                using var httpClient = new HttpClient();
                                byte[] oggData = await httpClient.GetByteArrayAsync(url);

                                string result;

                                try
                                {
                                    byte[] wavData = AudioConverter.ConvertOggToWav(oggData);
                                    result = await new WhisperRecognizer(_openaiToken).TranscribeAsync(wavData, "voice.wav");
                                }
                                catch (Exception ex) when (
                                    ex.Message.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                                    ex.Message.Contains("invalid api key", StringComparison.OrdinalIgnoreCase))
                                {
                                    result = await new WitAiRecognizer(_witAiToken).TranscribeAsync(oggData, "voice.ogg");
                                }
                                catch (Exception ex)
                                {
                                    result = $"❌ Ошибка при распознавании: {ex.Message}";
                                }

                                await botClient.SendMessage(chat.Id, $"🎤 Распознанный текст:\n\n{result}");
                            }

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
