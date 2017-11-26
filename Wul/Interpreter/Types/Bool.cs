using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    class Bool : IValue
    {
        private static readonly BoolMetaType metaType = new BoolMetaType();

        public static Bool True = new Bool(true);
        public static Bool False = new Bool(false);

        public MetaType ValueMetaType { get; set; }
        public MetaType MetaType => ValueMetaType;

        public readonly bool Value;

        private Bool(bool value)
        {
            Value = value;
            ValueMetaType = metaType;
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
