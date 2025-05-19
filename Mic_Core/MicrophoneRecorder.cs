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

        public void StartRecording()
        {
            stream = new MemoryStream();
            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1)
            };

            writer = new WaveFileWriter(stream, waveIn.WaveFormat);
            waveIn.DataAvailable += (s, e) =>
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
                writer.Flush();
            };

            waveIn.RecordingStopped += (s, e) =>
            {
                writer?.Dispose();
                byte[] data = stream!.ToArray();
                OnRecordingComplete?.Invoke(data);
            };

            waveIn.StartRecording();
        }

        public void StopRecording()
        {
            waveIn?.StopRecording();
            waveIn?.Dispose();
        }
    }
}
