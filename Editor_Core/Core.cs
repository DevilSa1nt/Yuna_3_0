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

                string newCode = Regex.Replace(code, pattern, replacement);

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

        public bool ReplaceInProjectFile(string solutionRoot, string projectName, string fileName, string pattern, string replacement, out string message)
        {
            string filePath = Path.Combine(solutionRoot, projectName, fileName);
            return ReplaceInFile(filePath, pattern, replacement, out message);
        }
    }
}
