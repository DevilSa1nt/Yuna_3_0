using System.Diagnostics;
using System.Text;

namespace Git_Core
{
    public class Core
    {
        public static bool CommitAndPush(string repoPath, string commitMessage, out string output)
        {
            output = "";

            if (string.IsNullOrWhiteSpace(repoPath) || !Directory.Exists(repoPath))
            {
                output = "❌ Путь к репозиторию не существует.";
                return false;
            }

            try
            {
                output += RunGit("add .", repoPath);
                output += RunGit($"commit -m \"{commitMessage}\"", repoPath);
                output += RunGit("push", repoPath);
                return true;
            }
            catch (Exception ex)
            {
                output += $"❌ Исключение: {ex.Message}";
                return false;
            }
        }

        public static string RunGit(string arguments, string workingDirectory)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var builder = new StringBuilder();
            process.OutputDataReceived += (_, e) => builder.AppendLine(e.Data);
            process.ErrorDataReceived += (_, e) => builder.AppendLine(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            return builder.ToString();
        }

        // <<<COMMANDS>>>
    }
}
