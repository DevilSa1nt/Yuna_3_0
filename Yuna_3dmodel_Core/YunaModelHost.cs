using System.Windows;

namespace Yuna_3dmodel_Core
{
    public static class YunaModelHost
    {
        private static YunaOverlayWindow _window;

        public static void Show()
        {
            if (_window != null)
                return;

            _window = new YunaOverlayWindow();
            _window.Show();
        }

        public static void Hide()
        {
            _window?.Close();
            _window = null;
        }
    }
}
