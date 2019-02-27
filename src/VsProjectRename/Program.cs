using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using CommandLine;

namespace VsProjectRename
{
    class Program
    {
        private static int _replaceInFilesCount;
        private static int _replaceInFileNamesCount;
        private static int _replaceInDirectoryNamesCount;

        static void Main(string[] args)
        {
            Console.WriteLine("Working... \n");

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

                ReplaceInFiles(directoryPath, opts.FindText, opts.ReplaceText, deleteVsUserSettingsDirectory: true);

                ReplaceInFileNames(directoryPath, opts.FindText, opts.ReplaceText, deleteVsUserSettingsDirectory: true);

                ReplaceInDirectoryNames(directoryPath, opts.FindText, opts.ReplaceText, deleteVsUserSettingsDirectory: true);

                Console.WriteLine($"Finished.\n");

                Console.WriteLine($"Replaced {_replaceInFilesCount} occurrences in files");
                Console.WriteLine($"Replaced {_replaceInFileNamesCount} occurrences in file names");
                Console.WriteLine($"Replaced {_replaceInDirectoryNamesCount} occurrences in directory names");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed with exception: {ex.Message}");
            }
        }

        public static void ReplaceInFiles(string directoryPath, string findText, string replaceText, bool deleteVsUserSettingsDirectory)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            if (deleteVsUserSettingsDirectory && directoryInfo.Name == ".vs")
            {
                Directory.Delete(directoryInfo.FullName, true);

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

            if (deleteVsUserSettingsDirectory && directoryInfo.Name == ".vs")
            {
                Directory.Delete(directoryInfo.FullName, true);

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

            if(deleteVsUserSettingsDirectory && directoryInfo.Name == ".vs")
            {
                Directory.Delete(directoryInfo.FullName, true);

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
