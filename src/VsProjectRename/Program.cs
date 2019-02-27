using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using CommandLine;

namespace VsProjectRename
{
    class Program
    {
        private static string[] _ignoredDirectoryNames = { ".vs", ".git", ".svn" };

        private static int _replaceInFilesCount;
        private static int _replaceInFileNamesCount;
        private static int _replaceInDirectoryNamesCount;

        static void Main(string[] args)
        {
            string ignoredFolders = _ignoredDirectoryNames.Aggregate<string, string>(null, (current, ignoredDirectoryName) => current + $"'{ignoredDirectoryName}', ").TrimEnd(new char[] {',', ' '});

            Console.WriteLine($"This application is best run with administrator privileges. Directories with the following names will be ignored to try and prevent access / corruption issues: {ignoredFolders}");

            // Using commandline parser to 
            // https://github.com/commandlineparser/commandline

            Parser.Default
                  .ParseArguments<CommandLineOptions>(args)
                  .WithParsed(RunOptionsAndReturnExitCode)
                  .WithNotParsed(HandleParseError);

#if DEBUG
            Console.ReadLine();
#endif
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var error in errs)
            {
                Console.WriteLine(error.ToString());
            }
        }

        private static void RunOptionsAndReturnExitCode(CommandLineOptions opts)
        {
            try
            {
                string directoryPath = opts.DirectoryPath;
                string findText = opts.FindText;
                string replaceText = opts.ReplaceText;

                if(string.IsNullOrWhiteSpace(directoryPath))
                {
                    Console.WriteLine("\nEnter root directory path:");
                    directoryPath = Console.ReadLine();
                }
                else

                {
                    Console.WriteLine($"\nRoot directory path: {directoryPath}");
                }

                if (string.IsNullOrWhiteSpace(findText))
                {
                    Console.WriteLine("\nEnter find text (case sensitive):");
                    findText = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"\nFind text: {directoryPath}");
                }

                if (string.IsNullOrWhiteSpace(replaceText))
                {
                    Console.WriteLine("\nEnter replace text (case sensitive):");
                    replaceText = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"\nReplace text: {directoryPath}");
                }

                Console.WriteLine("\nWorking...");

                ReplaceInFiles(directoryPath, findText, replaceText, deleteVsUserSettingsDirectory: true);

                ReplaceInFileNames(directoryPath, findText, replaceText, deleteVsUserSettingsDirectory: true);

                ReplaceInDirectoryNames(directoryPath, findText, replaceText, deleteVsUserSettingsDirectory: true);

                Console.WriteLine($"\nFinished.\n");

                Console.WriteLine($"Replaced {_replaceInFilesCount} occurrences in files");
                Console.WriteLine($"Replaced {_replaceInFileNamesCount} occurrences in file names");
                Console.WriteLine($"Replaced {_replaceInDirectoryNamesCount} occurrences in directory names");

                Console.WriteLine($"\nPress enter to continue.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed with exception: {ex.Message}");
            }

            Console.ReadLine();
        }

        public static void ReplaceInFiles(string directoryPath, string findText, string replaceText, bool deleteVsUserSettingsDirectory)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            if (deleteVsUserSettingsDirectory && _ignoredDirectoryNames.Contains(directoryInfo.Name))
            {
                return;
            }

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
                ReplaceInFiles(directory, findText, replaceText, deleteVsUserSettingsDirectory);
            }
        }

        public static void ReplaceInFileNames(string directoryPath, string findText, string replaceText, bool deleteVsUserSettingsDirectory)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            if (deleteVsUserSettingsDirectory && _ignoredDirectoryNames.Contains(directoryInfo.Name))
            {
                return;
            }

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
                ReplaceInFileNames(directory, findText, replaceText, deleteVsUserSettingsDirectory);
            }
        }

        public static void ReplaceInDirectoryNames(string directoryPath, string findText, string replaceText, bool deleteVsUserSettingsDirectory)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            if (deleteVsUserSettingsDirectory && _ignoredDirectoryNames.Contains(directoryInfo.Name))
            {
                return;
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
                ReplaceInDirectoryNames(directory, findText, replaceText, deleteVsUserSettingsDirectory);
            }
        }
    }
}
