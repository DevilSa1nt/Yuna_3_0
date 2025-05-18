using System.Net.Http.Headers;
using System.Text.Json;

namespace Voice_Core
{
    public class WitAiRecognizer : IVoiceRecognizer
    {
        private readonly string _accessToken;

        public WitAiRecognizer(string accessToken)
        {
            _accessToken = accessToken;
        }

        public async Task<string> TranscribeAsync(byte[] audioData, string fileName)
        {
            audioData = AudioConverter.ConvertOggToWav(audioData);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var content = new ByteArrayContent(audioData);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

            var response = await client.PostAsync("https://api.wit.ai/speech?v=20210928", content);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("text", out var text) ? text.GetString() : "[Нет текста]";
        }(byte[] audioData, string fileName)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var content = new ByteArrayContent(audioData);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

            var response = await client.PostAsync("https://api.wit.ai/speech?v=20210928", content);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("text", out var text) ? text.GetString() : "[��� ������]";
        }
    }
}
