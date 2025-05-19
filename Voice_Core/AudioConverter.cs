using System.Diagnostics;

namespace Voice_Core
{
    public static class AudioConverter
    {
        public static byte[] ConvertOggToWav(byte[] oggData)
        {
            string inputPath = Path.GetTempFileName() + ".ogg";
            string outputPath = Path.ChangeExtension(inputPath, ".wav");

            File.WriteAllBytes(inputPath, oggData);

            var processInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{inputPath}\" -ar 16000 -ac 1 \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            process.WaitForExit();

            if (!File.Exists(outputPath))
                throw new Exception("❌ ffmpeg не смог сконвертировать файл");

            byte[] wav = File.ReadAllBytes(outputPath);

            File.Delete(inputPath);
            File.Delete(outputPath);

            return wav;
        }
    }
}
