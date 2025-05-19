using OpenCvSharp;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Vision_Core
{
    public class RoboflowRecognizer : IGestureRecognizer
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

        public async Task<string> RecognizeAsync(Mat frame)
        {
            byte[] jpeg = frame.ToBytes(".jpg");
            string base64 = Convert.ToBase64String(jpeg);
            var payload = new StringContent(JsonConvert.SerializeObject(new { image = base64 }), Encoding.UTF8, "application/json");

            var url = $"{_endpoint}?api_key={_apiKey}";
            var response = await _httpClient.PostAsync(url, payload);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : $"Error: {response.StatusCode}";
        }
    }
}
