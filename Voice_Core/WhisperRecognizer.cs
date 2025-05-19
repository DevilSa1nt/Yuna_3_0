using System.Net.Http.Headers;
using System.Text.Json;

namespace Voice_Core
{
    public class WhisperRecognizer : IVoiceRecognizer
    {
        private readonly string _apiKey;

        public WhisperRecognizer(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> TranscribeAsync(byte[] audioData, string fileName)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            using var content = new MultipartFormDataContent();
            var audioContent = new ByteArrayContent(audioData);
            audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");
            content.Add(audioContent, "file", fileName);
            content.Add(new StringContent("whisper-1"), "model");

            var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", content);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("error", out var err))
            {
                var msg = err.GetProperty("message").GetString();
                throw new Exception(msg ?? "Unknown Whisper error");
            }

            return doc.RootElement.GetProperty("text").GetString();
        }
    }
}
