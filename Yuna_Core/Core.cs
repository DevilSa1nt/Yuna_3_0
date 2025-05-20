using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;

using TgBot_Core;
using Mic_Core;
using Vision_Core;
using System.Windows.Media.Imaging;
using Yuna_3dmodel_Core_WPF;

namespace Yuna_Core
{
    public class Core
    {
        public VisionCore Vision { get; private set; }

        private TgBot_Core.Core tgBotCore;
        private MicManager _mic;

        // 🔹 Событие, которое будет слушать MainWindow
        public event Action<BitmapSource> OnCameraFrame;

        public async Task InitAsync()
        {
            tgBotCore = new(AppConfig.TgToken, AppConfig.OpenAiKey, AppConfig.WitAiToken);
            tgBotCore.RestartT += RestartApplication;

            var yunaWindow = new YunaOverlayWindow();
            yunaWindow.Show();

            _mic = new(AppConfig.OpenAiKey, AppConfig.WitAiToken);

            Vision = new(AppConfig.Configuration);

            // 🔄 Проксируем кадры от VisionCore в наше событие
            Vision.OnFrameReady += bmp => OnCameraFrame?.Invoke(bmp);

            //await Task.CompletedTask;
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
                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true
                });

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
