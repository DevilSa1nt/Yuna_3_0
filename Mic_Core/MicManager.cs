using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Voice_Core;

namespace Mic_Core
{
    public class MicManager
    {
        private readonly MicrophoneRecorder recorder = new();
        private readonly string openAiKey;
        private readonly string witAiKey;

        private bool _isSpeaking = false;
        private DateTime _lastVoiceTime;
        private float _vadThreshold = 0.02f; // громкость
        private TimeSpan _silenceTimeout = TimeSpan.FromMilliseconds(1500);

        public MicManager(string openAiKey, string witAiKey)
        {
            this.openAiKey = openAiKey;
            this.witAiKey = witAiKey;

            recorder.OnAudioLevel += OnAudioLevel;
            recorder.OnRecordingComplete += OnRecordingComplete;

            Console.WriteLine("▶️ VAD микрофон запущен...");
            recorder.Start();

            Task.Run(() => VoiceLoop());
        }

        private void OnAudioLevel(float level)
        {
            if (level >= _vadThreshold)
            {
                _lastVoiceTime = DateTime.UtcNow;
                if (!_isSpeaking)
                {
                    Console.WriteLine("🟢 Говорю...");
                    _isSpeaking = true;
                }
            }
        }

        private async Task VoiceLoop()
        {
            while (true)
            {
                if (_isSpeaking && (DateTime.UtcNow - _lastVoiceTime > _silenceTimeout))
                {
                    Console.WriteLine("🔴 Замолчал, останавливаем запись...");
                    _isSpeaking = false;
                    recorder.Stop();
                }

                await Task.Delay(200);
            }
        }

        private async void OnRecordingComplete(byte[] wav)
        {
            Console.WriteLine($"📦 WAV готов: {wav.Length} байт");
            File.WriteAllBytes("vad_output.wav", wav);

            if (wav.Length < 1000)
            {
                Console.WriteLine("⚠️ Слишком мало данных для распознавания");
                recorder.Start(); // запускаем снова
                return;
            }

            try
            {
                string text = await new WhisperRecognizer(openAiKey).TranscribeAsync(wav, "mic.wav");
                Console.WriteLine("🎙 Whisper → " + text);
            }
            catch (Exception ex) when (
                ex.Message.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("invalid api key", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string text = await new WitAiRecognizer(witAiKey).TranscribeAsync(wav, "mic.wav");
                    Console.WriteLine("🎙 Wit.ai → " + text);
                }
                catch (Exception witEx)
                {
                    Console.WriteLine("❌ Wit.ai ошибка: " + witEx.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Whisper ошибка: " + ex.Message);
            }

            recorder.Start(); // запись следующего фрагмента
        }
    }
}
    