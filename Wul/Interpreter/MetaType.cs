using System;
using System.Collections.Generic;
using System.Linq;

namespace Wul.Interpreter
{
    public class MetaMethod 
    {
        public IFunction Method
        {
            get;
            set;
        }

        public bool IsDefined => Method != null;

        public string Name { get; }

        public MetaMethod(string name)
        {
            Name = name;
        }

        public IValue Invoke(List<IValue> arguments, Scope s)
        {
            IValue lhs = arguments.First();
            //TODO Magic metamethods, e.g. at
            return Method?.Evaluate(arguments, s) ?? throw new NotSupportedException($"Unable to invoke metamethod `{Name}` on type {lhs.GetType().Name}");
        }
    }

    public abstract class MetaType
    {
        protected MetaType()
        {
            Add = new MetaMethod("+");
            Subtract = new MetaMethod("-");
            Multiply = new MetaMethod("*");
            Divide = new MetaMethod("/");
            Modulus = new MetaMethod("%");
            Power = new MetaMethod("**");

            Not = new MetaMethod("not");
            And = new MetaMethod("and");
            Or = new MetaMethod("or");
            Xor = new MetaMethod("xor");

            Equal = new MetaMethod("=");
            Compare = new MetaMethod("compare");

            At = new MetaMethod("@");
            Remainder = new MetaMethod("rem");
            Count = new MetaMethod("len");
            Concat = new MetaMethod("..");

            Invoke = new MetaMethod("()");
            InvokeMagic = new MetaMethod("()");
            AsString = new MetaMethod("string");
        }

        // Arithmetic
        public MetaMethod Add { get; }
        public MetaMethod Subtract { get; }
        public MetaMethod Multiply { get; }
        public MetaMethod Divide { get; }
        public MetaMethod Modulus { get; }
        public MetaMethod Power { get; }
        //public MetaMethod IntegerDivide { get; }
        
        // Logical
        public MetaMethod Not { get; }
        public MetaMethod And { get; }
        public MetaMethod Or { get; }
        public MetaMethod Xor { get; }

        // Bitwise

        // Comparison
        public MetaMethod Equal { get; }
        public MetaMethod Compare { get; }

        // List
        public MetaMethod At { get; }
        public MetaMethod Remainder { get; }
        public MetaMethod Count { get; }
        public MetaMethod Concat { get; }

        // Other
        public MetaMethod Invoke { get; }
        public MetaMethod InvokeMagic { get; }
        public MetaMethod AsString { get; }

        protected IValue IdentityEqual (List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            IValue second = arguments.Skip(1).First();
            return first.Equals(second) ? Bool.True : Bool.False;
        }

        protected IValue IdentityString(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            return new UString(first.AsString());
        }
    }
}
