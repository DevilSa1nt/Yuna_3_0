using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor_Core
{
    public class SolutionManager
    {
        private readonly string _solutionRoot;

        public SolutionManager(string solutionRoot)
        {
            _solutionRoot = solutionRoot;
        }

        public bool CreateNewProject(string name, out string output)
        {
            string projectPath = Path.Combine(_solutionRoot, name);

            if (Directory.Exists(projectPath))
            {
                output = "❗ Проект уже существует.";
                return false;
            }

            output = Run("dotnet", $"new classlib -n {name}", _solutionRoot);
            output += Run("dotnet", $"sln add {name}/{name}.csproj", _solutionRoot);
            return true;
        }

        public bool AddProjectReference(string fromProject, string toProject, out string output)
        {
            string fromPath = Path.Combine(_solutionRoot, fromProject, $"{fromProject}.csproj");
            string toPath = Path.Combine(_solutionRoot, toProject, $"{toProject}.csproj");

            if (!File.Exists(fromPath) || !File.Exists(toPath))
            {
                output = "❌ Один из проектов не найден.";
                return false;
            }

            output = Run("dotnet", $"add \"{fromPath}\" reference \"{toPath}\"", _solutionRoot);
            return true;
        }

        private string Run(string file, string args, string workingDir)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file,
                    Arguments = args,
                    WorkingDirectory = workingDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            p.Start();
            string result = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
            p.WaitForExit();
            return result;
        }

        // <<<EXTENSIONS>>>
    }
}
