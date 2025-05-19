using System;
using System.Threading.Tasks;
using Voice_Core;

namespace Mic_Core
{
    public class MicManager
    {
        private readonly MicrophoneRecorder recorder = new();
        private readonly string openAiKey;
        private readonly string witAiKey;

        public MicManager(string openAiKey, string witAiKey)
        {
            this.openAiKey = openAiKey;
            this.witAiKey = witAiKey;
        }

        public void Start()
        {
            recorder.OnRecordingComplete += async (wav) =>
            {
                try
                {
                    string text = await new WhisperRecognizer(openAiKey).TranscribeAsync(wav, "mic.wav");
                    Console.WriteLine("🎙 Whisper → " + text);
                }
                catch (Exception ex) when (
                    ex.Message.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("invalid api key", StringComparison.OrdinalIgnoreCase))
                {
                    string text = await new WitAiRecognizer(witAiKey).TranscribeAsync(wav, "mic.wav");
                    Console.WriteLine("🎙 Wit.ai → " + text);
                }

                await Task.Delay(500);
                recorder.StartRecording();
            };

            recorder.StartRecording();
            Console.WriteLine("▶️ Микрофон запущен...");
        }

        public void Stop() => recorder.StopRecording();
    }
}
