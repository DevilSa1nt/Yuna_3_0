using System.Net.Http.Headers;

namespace Voice_Core
{
    public class WitAiRecognizer : IVoiceRecognizer
    {
        private readonly string _accessToken;

        public WitAiRecognizer(string accessToken)
        {
