using System.Text.RegularExpressions;

namespace Editor_Core
{
    public class Core
    {
        public bool ReplaceInFile(string filePath, string pattern, string replacement, out string message)
        {
            message = "";

            if (!File.Exists(filePath))
            {
                message = "Файл не найден.";
                return false;
            }

            try
            {
                string code = File.ReadAllText(filePath);

                // Расшифровка управляющих символов (\n -> перенос строки и т.д.)
                replacement = Unescape(replacement);

                string newCode = code.Replace(pattern, replacement);
                File.WriteAllText(filePath, newCode);
                message = "Файл успешно обновлён.";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Ошибка редактирования: {ex.Message}";
                return false;
            }
        }

        private string Unescape(string s)
        {
            return s
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t")
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\");
        }

        public bool ReplaceInProjectFile(string solutionRoot, string projectName, string fileName, string pattern, string replacement, out string message)
        {
            string filePath = Path.Combine(solutionRoot, projectName, fileName);
            return ReplaceInFile(filePath, pattern, replacement, out message);
        }

        public bool SaveFileToProject(string solutionRoot, string projectName, string fileName, string rawCode, out string message)
        {
            try
            {
                string code = Unescape(rawCode);

                string filePath = Path.Combine(solutionRoot, projectName, fileName);
                File.WriteAllText(filePath, code);

                message = $"✅ Файл сохранён: {projectName}/{fileName}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"❌ Ошибка при сохранении файла: {ex.Message}";
                return false;
            }
        }
    }
}
