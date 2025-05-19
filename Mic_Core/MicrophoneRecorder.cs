using System;
using System.IO;
using NAudio.Wave;

namespace Mic_Core
{
    public class MicrophoneRecorder
    {
        private WaveInEvent? waveIn;
        private MemoryStream? stream;
        private WaveFileWriter? writer;

        public event Action<byte[]>? OnRecordingComplete;
        public event Action<float>? OnAudioLevel; // для VAD

        public void Start()
        {
            stream = new MemoryStream();
            waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(16000, 1),
                BufferMilliseconds = 100
            };

            writer = new WaveFileWriter(stream, waveIn.WaveFormat);

            waveIn.DataAvailable += (s, e) =>
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
                writer.Flush();

                float rms = CalculateRms(e.Buffer, e.BytesRecorded);
                OnAudioLevel?.Invoke(rms);
            };

            waveIn.RecordingStopped += (s, e) =>
            {
                writer?.Dispose();
                byte[] data = stream!.ToArray();
                OnRecordingComplete?.Invoke(data);
            };

            waveIn.StartRecording();
        }

        public void Stop()
        {
            waveIn?.StopRecording();
            waveIn?.Dispose();
        }

        private float CalculateRms(byte[] buffer, int bytesRecorded)
        {
            float sum = 0;
            for (int i = 0; i < bytesRecorded; i += 2)
            {
                short sample = BitConverter.ToInt16(buffer, i);
                sum += sample * sample;
            }

            float rms = (float)Math.Sqrt(sum / (bytesRecorded / 2));
            return rms / short.MaxValue;
        }
    }
}
