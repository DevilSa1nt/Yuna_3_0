using OpenAI.Chat;
using System;

namespace AI_Core
{
    public class Core
    {
        private readonly ChatClient _client;

        public Core(string apiKey)
        {
            _client = new ChatClient("gpt-4.1-mini", apiKey);
        }

        public string GetUpdatedCode(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return "";

            ChatCompletion completion = _client.CompleteChat(prompt);
            return completion?.Content?[0]?.Text ?? "";
        }
    }
}