using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

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

        public MetaMethod(MetaMethod other)
        {
            Method = other.Method;
            Name = other.Name;
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
            InvokeMagic = new MetaMethod("@()");
            AsString = new MetaMethod("string");
            Type = new MetaMethod("type");

            InitializeDictionary();
        }

        //Surely there is a better way
        public MetaType Clone()
        {
            var clone = (MetaType) MemberwiseClone();

            clone.Add = new MetaMethod(Add);
            clone.Subtract = new MetaMethod(Subtract);
            clone.Multiply = new MetaMethod(Multiply);
            clone.Divide = new MetaMethod(Divide);
            clone.Modulus = new MetaMethod(Modulus);
            clone.Power = new MetaMethod(Power);

            clone.Not = new MetaMethod(Not);
            clone.And = new MetaMethod(And);
            clone.Or = new MetaMethod(Or);
            clone.Xor = new MetaMethod(Xor);

            clone.Equal = new MetaMethod(Equal);
            clone.Compare = new MetaMethod(Compare);

            clone.At = new MetaMethod(At);
            clone.Remainder = new MetaMethod(Remainder);
            clone.Count = new MetaMethod(Count);
            clone.Concat = new MetaMethod(Concat);

            clone.Invoke = new MetaMethod(Invoke);
            clone.InvokeMagic = new MetaMethod(InvokeMagic);
            clone.AsString = new MetaMethod(AsString);
            clone.Type = new MetaMethod(Type);

            clone.InitializeDictionary();

            return clone;
        }

        private void InitializeDictionary()
        {
            var metaMethodList = new List<MetaMethod>
            {
                Add, Subtract, Multiply, Divide, Modulus, Power,
                Not, And, Or, Xor,
                Equal, Compare,
                At, Remainder, Count, Concat,
                Invoke, InvokeMagic, AsString, Type
            };

            _metaMethods = metaMethodList.ToDictionary(key => key.Name);
        }

        private Dictionary<string, MetaMethod> _metaMethods;

        public MetaMethod Get(string name)
        {
            _metaMethods.TryGetValue(name, out MetaMethod metaMethod);
            return metaMethod;
        }

        // Arithmetic
        public MetaMethod Add { get; private set; }
        public MetaMethod Subtract { get; private set; }
        public MetaMethod Multiply { get; private set; }
        public MetaMethod Divide { get; private set; }
        public MetaMethod Modulus { get; private set; }
        public MetaMethod Power { get; private set; }
        //public MetaMethod IntegerDivide { get; private set; }
        
        // Logical
        public MetaMethod Not { get; private set; }
        public MetaMethod And { get; private set; }
        public MetaMethod Or { get; private set; }
        public MetaMethod Xor { get; private set; }

        // Bitwise
        //TODO

        // Comparison
        public MetaMethod Equal { get; private set; }
        public MetaMethod Compare { get; private set; }

        // List
        public MetaMethod At { get; private set; }
        public MetaMethod Remainder { get; private set; }
        public MetaMethod Count { get; private set; }
        public MetaMethod Concat { get; private set; }

        // Other
        public MetaMethod Invoke { get; private set; }
        public MetaMethod InvokeMagic { get; private set; }
        public MetaMethod AsString { get; private set; }
        public MetaMethod Type { get; private set; }

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

        protected IValue IdentityType(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            return first.Type;
        }
    }
}
