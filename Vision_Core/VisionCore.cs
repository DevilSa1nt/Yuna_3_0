using Microsoft.Extensions.Configuration;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Vision_Core
{
    public class VisionCore
    {
        private readonly IGestureRecognizer _recognizer;
        private bool _enabled = true;

        public event Action<BitmapSource> OnFrameReady;

        public VisionCore(IConfiguration config)
        {
            var section = config.GetSection("Vision");
            var url = section.GetValue<string>("PythonApiUrl");

            _recognizer = new PythonHandRecognizer(url);
            _ = Task.Run(VideoLoop);
        }

        private async Task VideoLoop()
        {
            using var capture = new VideoCapture(0);
            using var frame = new Mat();

            while (_enabled)
            {
                capture.Read(frame);
                if (frame.Empty()) continue;

                // 🔁 Зеркалирование
                Cv2.Flip(frame, frame, FlipMode.Y);

                try
                {
                    var hands = await _recognizer.RecognizeHandsAsync(frame);

                    foreach (var hand in hands)
                        DrawHandSkeleton(frame, hand);

                    var bmp = ConvertMatToBitmapSource(frame);
                    bmp.Freeze();
                    OnFrameReady?.Invoke(bmp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VisionCore] Ошибка: {ex.Message}");
                }

                await Task.Delay(50);
            }
        }

        private void DrawHandSkeleton(Mat frame, HandData hand)
        {
            int w = frame.Width;
            int h = frame.Height;

            var color = hand.Label == "Right" ? Scalar.Cyan : Scalar.Magenta;
            var keypoints = hand.Keypoints;

            if (keypoints == null || keypoints.Count == 0) return;

            // 🔹 Нарисовать точки
            for (int i = 0; i < keypoints.Count; i++)
            {
                var p = keypoints[i];
                int x = (int)(p.X * w);
                int y = (int)(p.Y * h);
                Cv2.Circle(frame, new OpenCvSharp.Point(x, y), 4, color, -1);
            }

            // 🔹 Нарисовать кости
            var connections = new List<(int, int)>
            {
                (0, 1), (1, 2), (2, 3), (3, 4),
                (0, 5), (5, 6), (6, 7), (7, 8),
                (0, 9), (9,10), (10,11), (11,12),
                (0,13), (13,14), (14,15), (15,16),
                (0,17), (17,18), (18,19), (19,20)
            };

            foreach (var (start, end) in connections)
            {
                if (start < keypoints.Count && end < keypoints.Count)
                {
                    var p1 = keypoints[start];
                    var p2 = keypoints[end];

                    int x1 = (int)(p1.X * w);
                    int y1 = (int)(p1.Y * h);
                    int x2 = (int)(p2.X * w);
                    int y2 = (int)(p2.Y * h);

                    Cv2.Line(frame, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), color, 2);
                }
            }

            // 🔹 Подпись над запястьем
            var wrist = keypoints[0];
            int tx = (int)(wrist.X * w);
            int ty = (int)(wrist.Y * h) - 10;
            Cv2.PutText(frame, hand.Label, new OpenCvSharp.Point(tx, ty),
                HersheyFonts.HersheySimplex, 0.6, color, 2);

            // 🔹 Расстояние между большим (4) и указательным (8)
            if (keypoints.Count > 8)
            {
                var thumbTip = keypoints[4];
                var indexTip = keypoints[8];

                int x1 = (int)(thumbTip.X * w);
                int y1 = (int)(thumbTip.Y * h);
                int x2 = (int)(indexTip.X * w);
                int y2 = (int)(indexTip.Y * h);

                double distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
                string distText = $"{distance:F0}px";

                // Центр между пальцами
                int midX = (x1 + x2) / 2;
                int midY = (y1 + y2) / 2;

                Cv2.PutText(frame, distText, new OpenCvSharp.Point(midX, midY - 10),
                    HersheyFonts.HersheySimplex, 0.55, color, 2);
            }
        }

        private static BitmapSource ConvertMatToBitmapSource(Mat image)
        {
            using var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
                bitmap.Dispose();
            }
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
