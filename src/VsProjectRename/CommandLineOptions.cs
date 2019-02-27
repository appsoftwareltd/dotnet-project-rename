using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace VsProjectRename
{
    public class CommandLineOptions
    {
        [Option('d', "directory-path", Required = true, HelpText = "Directory path")]
        public string DirectoryPath { get; set; }

        [Option('f', "find-text", Required = true, HelpText = "Find text")]
        public string FindText { get; set; }

        [Option('r', "replace-text", Required = true, HelpText = "Replace text")]
        public string ReplaceText { get; set; }
    }
}
