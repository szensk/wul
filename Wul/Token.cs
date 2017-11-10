namespace Wul
{
    class Token
    {
        public string Value { get; }
        public string File { get; }
        public int Line { get; }
        public int Column { get; }

        public Token(string value, string file, int line, int column)
        {
            Value = value;
            File = file;
            Line = line;
            Column = column;
        }
    }
}