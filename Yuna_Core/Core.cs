using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.IO;
using Microsoft.Extensions.Configuration;

using TgBot_Core;
using Mic_Core;

namespace Yuna_Core
{
    public class Core
    {
        TgBot_Core.Core tgBotCore;
        Mic_Core.MicManager _mic;

        public Core()
        {
            Work();
        }

        async Task Work()
        {
            tgBotCore = new(AppConfig.TgToken, AppConfig.OpenAiKey, AppConfig.WitAiToken);
            tgBotCore.RestartT += RestartApplication;

            _mic = new(AppConfig.OpenAiKey, AppConfig.WitAiToken);
        }

        public static void RestartApplication()
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;

            if (!File.Exists(exePath))
            {
                MessageBox.Show($"Файл не найден: {exePath}");
                return;
            }

            try
            {
                // Запускаем новое приложение
                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true
                });

                // Корректное завершение приложения из UI-потока
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при перезапуске: " + ex.Message);
            }

        }
    }

    public static class AppConfig
    {
        public static IConfigurationRoot Configuration { get; }

        static AppConfig()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("E:\\Yuna_3_0\\appsettings.Development.json")
                .Build();
        }

        public static string TgToken => Configuration["Bot:Token"];
        public static string OpenAiKey => Configuration["OpenAI:ApiKey"];
        public static string[] AllowedUserIds =>
            Configuration.GetSection("Access:AllowedUserIds").Get<string[]>();
        public static string WitAiToken => Configuration["WitAI:Token"];

    }
}
