using System;
using System.Diagnostics;
using System.Text;

namespace Editor_Core
{
    public class ProjectBuilder
    {
        public bool BuildProject(string csprojPath, out string buildOutput)
        {
            buildOutput = "";

            if (string.IsNullOrWhiteSpace(csprojPath) || !System.IO.File.Exists(csprojPath))
            {
                buildOutput = "Файл проекта не найден.";
                return false;
            }

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"build \"{csprojPath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                var outputBuilder = new StringBuilder();
                process.OutputDataReceived += (sender, e) => outputBuilder.AppendLine(e.Data);
                process.ErrorDataReceived += (sender, e) => outputBuilder.AppendLine(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                buildOutput = outputBuilder.ToString();
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                buildOutput = $"Ошибка сборки: {ex.Message}";
                return false;
            }
        }
    }
}
