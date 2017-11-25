using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    class Bool : IValue
    {
        public static Bool True = new Bool(true);
        public static Bool False = new Bool(false);

        private static readonly BoolMetaType metaType = new BoolMetaType();
        public MetaType MetaType => metaType;

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

        public object ToObject()
        {
            return Value;
        }
    }
}
