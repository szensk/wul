using System;

namespace Wul.Interpreter.Types
{
    abstract class Value : IValue
    {
        //TODO do we want a nil meta type?
        public MetaType MetaType => null;

        public virtual string AsString()
        {
            throw new NotImplementedException();
        }

        public virtual object ToObject()
        {
            throw new NotImplementedException();
        }

        public static Value Nil = new Nill();
    }

    internal class Nill : Value
    {
        public override string AsString()
        {
            return "nil";
        }

        public override object ToObject()
        {
            return null;
        }
    }
}