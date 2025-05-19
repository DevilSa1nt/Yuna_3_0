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
            var raw = await response.Content.ReadAsStringAsync();

            try
            {
                // Ищем последнюю строку "text": "..."
                var matches = System.Text.RegularExpressions.Regex.Matches(raw, "\"text\"\\s*:\\s*\"([^\"]+)\"");
                if (matches.Count > 0)
                {
                    var lastText = matches[^1].Groups[1].Value;
                    return lastText;
                }

                return "[Wit.ai не вернул текст]";
            }
            catch (Exception ex)
            {
                return $"❌ Ошибка при обработке ответа:\n{ex.Message}\n\nСырой ответ:\n{raw}";
            }
        }
    }
}
