using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    public class BoolType : WulType
    {
        public BoolType() : base("Bool", typeof(Bool))
        {
        }

        public static readonly BoolType Instance = new BoolType();
    }

    class Bool : IValue
    {
        private static readonly BoolMetaType metaType = new BoolMetaType();

        public static Bool True = new Bool(true);
        public static Bool False = new Bool(false);

        public MetaType MetaType { get; set; }

        public readonly bool Value;

        private Bool(bool value)
        {
            Value = value;
            MetaType = metaType;
        }

        public static implicit operator bool(Bool b)
        {
            return b.Value;
        }

        public WulType Type => BoolType.Instance;

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
