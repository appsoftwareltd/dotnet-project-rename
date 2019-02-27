using System;
using System.Collections.Generic;
using System.Text;

namespace VsProjectRename
{
    public static class StringExtensions
    {
        public static string ReplaceLastOccurrence(this string source, string findText, string replaceText)
        {
            int startIndex = source.LastIndexOf(findText, StringComparison.Ordinal);

            return source.Remove(startIndex, findText.Length).Insert(startIndex, replaceText);
        }
    }
}
