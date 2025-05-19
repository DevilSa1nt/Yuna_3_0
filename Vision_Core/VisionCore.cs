using Microsoft.Extensions.Configuration;
using OpenCvSharp;

namespace Vision_Core
{
    public class VisionCore
    {
        private readonly IGestureRecognizer _recognizer;

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
                _recognizer = new LocalGestureRecognizer(modelPath);
            }
        }

        public async Task<string> RecognizeGestureAsync(Mat frame)
        {
            return await _recognizer.RecognizeAsync(frame);
        }
    }
}
