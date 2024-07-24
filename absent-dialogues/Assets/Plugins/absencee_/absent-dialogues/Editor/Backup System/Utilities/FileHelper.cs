using System.IO;

namespace com.absence.dialoguesystem.editor.backup.utilities
{
    public static class FileHelper
    {
        public static void WriteToFile(string jsonText, string fullPath)
        {
            File.WriteAllText(fullPath, jsonText);
        }

        public static string ReadFromFile(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return null;
            return File.ReadAllText(fullPath);
        }
    }
}