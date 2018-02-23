using System;

namespace Wul.Parser
{
    public class ParseException : Exception
    {
        private string File { get; }
        public int Line { get; }
        private int StartCharacter { get; }
        private int EndCharacter { get; }

        public ParseException(string file, int line, int startIndex, int endIndex, string message) : base(message)
        {
            File = file;
            Line = line;
            StartCharacter = startIndex;
            EndCharacter = endIndex;
        }

        public string GetUnderline => EndCharacter >= 0 ? new string(' ', EndCharacter) + "^" : null;

        public string GetErrorMessage 
        {
            get
            {
                string error = "Parse error:";
                if (!string.IsNullOrEmpty(File))
                {
                    error += $" {File}";
                }
                
                return $"{error} line {Line} {Message}";
            }
        }
    }
}
