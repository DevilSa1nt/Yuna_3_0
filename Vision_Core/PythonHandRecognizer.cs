using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Vision_Core
{
    public class PythonHandRecognizer : IGestureRecognizer
    {
        private readonly HttpClient _client;

        public PythonHandRecognizer(string serverUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(serverUrl) };
        }

        public async Task<List<HandData>> RecognizeHandsAsync(Mat frame)
        {
            var jpeg = frame.ToBytes(".jpg");
            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(jpeg)
            {
                Headers = { ContentType = MediaTypeHeaderValue.Parse("image/jpeg") }
            }, "file", "frame.jpg");

            try
            {
                var response = await _client.PostAsync("/track", content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<MultiHandResponse>(json);
                return result?.Hands ?? new List<HandData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Python] Ошибка: {ex.Message}");
                return new List<HandData>();
            }
        }
    }

    public class MultiHandResponse
    {
        [JsonProperty("hands")]
        public List<HandData> Hands { get; set; }
    }

    public class HandData
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("keypoints")]
        public List<Point3D> Keypoints { get; set; }
    }
}
