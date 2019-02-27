using System;
using System.IO;
using System.Text.RegularExpressions;

namespace VsProjectRename
{
    class Program
    {
        private static int _replaceInFilesCount;
        private static int _replaceInFileNamesCount;
        private static int _replaceInDirectoryNamesCount;

        static void Main(string[] args)
        {
            try
            {
                string directoryPath = "";

                ReplaceInFiles(directoryPath, "Test", "Test1");

                ReplaceInFileNames(directoryPath, "Test", "Test1");

                ReplaceInDirectoryNames(directoryPath, "Test", "Test1", deleteVsUserSettingsDirectory: true);

                Console.WriteLine($"Replaced {_replaceInFilesCount} occurrences in files");
                Console.WriteLine($"Replaced {_replaceInFileNamesCount} occurrences in file names");
                Console.WriteLine($"Replaced {_replaceInDirectoryNamesCount} occurrences in directory names");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed with exception: {ex.Message}");
            }
        }

        public static void ReplaceInFiles(string directoryPath, string findText, string replaceText)
        {
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                string fileText = File.ReadAllText(file);

                int count = Regex.Matches(fileText, findText, RegexOptions.None).Count;

                if (count > 0)
                {
                    string contents = fileText.Replace(findText, replaceText);

                    File.WriteAllText(file, contents);

                    _replaceInFilesCount += count;
                }
            }

            foreach (string directory in Directory.GetDirectories(directoryPath))
            {
                ReplaceInFiles(directory, findText, replaceText);
            }
        }

        public static void ReplaceInFileNames(string directoryPath, string findText, string replaceText)
        {
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                var fileInfo = new FileInfo(file);

                int count = Regex.Matches(fileInfo.Name, findText, RegexOptions.None).Count;

                if (count > 0)
                {
                    string newFileName = fileInfo.Name.Replace(findText, replaceText);

                    string newFullFileName = fileInfo.FullName.ReplaceLastOccurrence(fileInfo.Name, newFileName);

                    File.Move(fileInfo.FullName, newFullFileName);

                    _replaceInFileNamesCount += count;
                }
            }

            foreach (string directory in Directory.GetDirectories(directoryPath))
            {
                ReplaceInFileNames(directory, findText, replaceText);
            }
        }

        public static void ReplaceInDirectoryNames(string directoryPath, string findText, string replaceText, bool deleteVsUserSettingsDirectory)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            if(deleteVsUserSettingsDirectory && directoryInfo.Name == ".vs")
            {
                Directory.Delete(directoryInfo.FullName);
            }

            int count = Regex.Matches(directoryInfo.Name, findText, RegexOptions.None).Count;

            string directoryInfoFullName = directoryInfo.FullName;

            if (count > 0)
            {
                string newDirectoryName = directoryInfo.Name.Replace(findText, replaceText);

                directoryInfoFullName = directoryInfo.FullName.ReplaceLastOccurrence(directoryInfo.Name, newDirectoryName);

                Directory.Move(directoryInfo.FullName, directoryInfoFullName);

                _replaceInDirectoryNamesCount += count;
            }

            foreach (string directory in Directory.GetDirectories(directoryInfoFullName))
            {
                ReplaceInDirectoryNames(directory, findText, replaceText, deleteVsUserSettingsDirectory: deleteVsUserSettingsDirectory);
            }
        }
    }
}
