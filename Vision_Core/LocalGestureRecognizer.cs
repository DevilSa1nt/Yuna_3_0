using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using Newtonsoft.Json;

namespace Vision_Core
{
    public class LocalGestureRecognizer : IGestureRecognizer
    {
        private readonly InferenceSession _session;

        public LocalGestureRecognizer(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }

        public Task<string> RecognizeAsync(Mat frame)
        {
            var resized = frame.Resize(new Size(224, 224));
            var inputTensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = resized.At<Vec3b>(y, x);
                    inputTensor[0, 0, y, x] = pixel.Item2 / 255.0f;
                    inputTensor[0, 1, y, x] = pixel.Item1 / 255.0f;
                    inputTensor[0, 2, y, x] = pixel.Item0 / 255.0f;
                }
            }

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor)
            };

            using var results = _session.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();

            return Task.FromResult(JsonConvert.SerializeObject(output));
        }
    }
}
