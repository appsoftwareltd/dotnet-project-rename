using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace VsProjectRename
{
    public class CommandLineOptions
    {
        [Option('d', "directory-path", Required = false, HelpText = "Directory path")]
        public string DirectoryPath { get; set; }

        [Option('f', "find-text", Required = false, HelpText = "Find text")]
        public string FindText { get; set; }

        [Option('r', "replace-text", Required = false, HelpText = "Replace text")]
        public string ReplaceText { get; set; }
    }
}
