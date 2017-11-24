﻿namespace Wul.Interpreter
{
    abstract class Value : IValue
    {
        //TODO do we want a nil meta type?
        public MetaType MetaType => null;

        public virtual string AsString()
        {
            throw new System.NotImplementedException();
        }

        public static Value Nil = new Nill();
    }

    internal class Nill : Value
    {
        public override string AsString()
        {
            return "nil";
        }
    }
}