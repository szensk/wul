namespace Wul.Interpreter
{
    internal class UString : IValue
    {
        public UString(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public string AsString()
        {
            return $"'{Value}'";
        }
    }
}