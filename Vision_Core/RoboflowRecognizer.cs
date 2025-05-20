using OpenCvSharp;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision_Core
{
    public class RoboflowRecognizer //: IGestureRecognizer
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public RoboflowRecognizer(string endpoint, string apiKey)
        {
            _endpoint = endpoint;
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<List<Point3D>> RecognizeAsync(Mat frame)
        {
            try
            {
                byte[] jpeg = frame.ToBytes(".jpg");
                string base64 = Convert.ToBase64String(jpeg);

                var payload = new StringContent(
                    JsonConvert.SerializeObject(new { image = base64 }),
                    Encoding.UTF8,
                    "application/json"
                );

                var url = $"{_endpoint}?api_key={_apiKey}";
                var response = await _httpClient.PostAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Roboflow] Ошибка запроса: {response.StatusCode}");
                    return new List<Point3D>();
                }

                string json = await response.Content.ReadAsStringAsync();

                // 👇 Пример: если ответ — просто массив чисел
                var floatArray = JsonConvert.DeserializeObject<float[]>(json);

                var keypoints = new List<Point3D>();
                for (int i = 0; i < floatArray.Length; i += 3)
                {
                    keypoints.Add(new Point3D
                    {
                        X = floatArray[i],
                        Y = floatArray[i + 1],
                        Z = floatArray[i + 2]
                    });
                }

                return keypoints;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Roboflow] Ошибка: {ex.Message}");
                return new List<Point3D>();
            }
        }
    }
}
