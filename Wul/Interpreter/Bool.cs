namespace Wul.Interpreter
{
    class Bool : IValue
    {
        public static Bool True = new Bool(true);
        public static Bool False = new Bool(false);

        public readonly bool Value;

        private Bool(bool value)
        {
            Value = value;
        }

        public static implicit operator bool(Bool b)
        {
            return b.Value;
        }

        public string AsString()
        {
            return $"{Value}";
        }
    }
}
