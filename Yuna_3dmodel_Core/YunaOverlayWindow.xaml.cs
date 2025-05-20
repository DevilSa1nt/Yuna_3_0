using SharpDX;
using System;
using System.IO;
using System.Windows;

namespace Yuna_3dmodel_Core
{
    public partial class YunaOverlayWindow : Window
    {
        private readonly YunaModel _model = new();

        public YunaOverlayWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.WorkArea.Right - Width - 20;
            Top = SystemParameters.WorkArea.Bottom - Height - 100;

            string path = "Assets/Models/Yuna.glb";

            if (!File.Exists(path))
            {
                MessageBox.Show($"Модель не найдена: {path}");
                return;
            }

            _model.Load(path);
            viewport.Items.Add(_model.ModelVisual);
        }
    }
}
