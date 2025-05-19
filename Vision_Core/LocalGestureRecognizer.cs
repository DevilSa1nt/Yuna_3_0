using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision_Core
{
    public class LocalGestureRecognizer : IGestureRecognizer
    {
        private readonly InferenceSession _session;
        private readonly string _inputName;

        public LocalGestureRecognizer(string modelPath)
        {
            _session = new InferenceSession(modelPath);
            _inputName = _session.InputMetadata.Keys.FirstOrDefault()
                ?? throw new Exception("ONNX модель не содержит входов.");
        }

        public async Task<List<Point3D>> RecognizeAsync(Mat frame)
        {
            var resized = frame.Resize(new Size(224, 224));
            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = resized.At<Vec3b>(y, x);
                    tensor[0, 0, y, x] = pixel.Item2 / 255.0f;
                    tensor[0, 1, y, x] = pixel.Item1 / 255.0f;
                    tensor[0, 2, y, x] = pixel.Item0 / 255.0f;
                }
            }

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(_inputName, tensor)
            };

            using var results = _session.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();

            var keypoints = new List<Point3D>();
            for (int i = 0; i < output.Length; i += 3)
            {
                keypoints.Add(new Point3D
                {
                    X = output[i],
                    Y = output[i + 1],
                    Z = output[i + 2]
                });
            }

            return await Task.FromResult(keypoints);
        }
    }
}
