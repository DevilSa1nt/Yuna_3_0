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
            bool useCloud = section.GetValue<bool>("UseCloud");

            if (useCloud)
            {
                var endpoint = section.GetValue<string>("RoboflowEndpoint");
                var apiKey = section.GetValue<string>("RoboflowApiKey");
                _recognizer = new RoboflowRecognizer(endpoint, apiKey);
            }
            else
            {
                var modelPath = section.GetValue<string>("LocalOnnxPath");
                _recognizer = new LocalGestureRecognizer("E:\\Yuna_3_0\\Models\\hand_landmark.onnx");
            }

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

                try
                {
                    var keypoints = await _recognizer.RecognizeAsync(frame);

                    DrawHandSkeleton(frame, keypoints);

                    var bmp = ConvertMatToBitmapSource(frame);
                    bmp.Freeze();
                    OnFrameReady?.Invoke(bmp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VisionCore] Ошибка: {ex.Message}");
                }

                await Task.Delay(100);
            }
        }

        private void DrawHandSkeleton(Mat frame, List<Point3D> keypoints)
        {
            var color = Scalar.LimeGreen;

            for (int i = 0; i < keypoints.Count; i++)
            {
                var p = keypoints[i];
                Cv2.Circle(frame, (int)p.X, (int)p.Y, 4, color, -1);
            }

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
                    Cv2.Line(frame, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, color, 2);
                }
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
