using System.Windows;
using System.Windows.Media.Imaging;
using Yuna_Core;

namespace Yuna_Core
{
    public partial class MainWindow : Window
    {
        private Core _core;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _core = new Core();
            await _core.InitAsync();

            // ✅ Подписываемся на событие, уже проксированное через Core
            _core.OnCameraFrame += UpdateCameraImage;
        }

        private void UpdateCameraImage(BitmapSource bitmap)
        {
            Dispatcher.Invoke(() =>
            {
                CameraImage.Source = bitmap;
            });
        }
    }
}
