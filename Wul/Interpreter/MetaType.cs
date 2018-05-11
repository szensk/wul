using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter
{
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
            IntegerDivide = new MetaMethod("//");

            Not = new MetaMethod("not");

            BitwiseNot = new MetaMethod("~");
            BitwiseAnd = new MetaMethod("&");
            BitwiseOr = new MetaMethod("|");
            BitwiseXor = new MetaMethod("^");
            LeftShift = new MetaMethod("<<");
            RightShift = new MetaMethod(">>");

            Equal = new MetaMethod("=");
            Compare = new MetaMethod("compare");

            At = new MetaMethod("at");
            Set = new MetaMethod("set");
            Remainder = new MetaMethod("rem");
            Count = new MetaMethod("len");
            Concat = new MetaMethod("..");
            Push = new MetaMethod("push");
            Pop = new MetaMethod("pop");
            Contains = new MetaMethod("contains?");

            Invoke = new MetaMethod("invoke");
            InvokeMagic = new MetaMethod("@invoke");
            ApplyMacro = new MetaMethod("apply");
            AsString = new MetaMethod("string");
            Type = new MetaMethod("type");

            Invoke.Method = new NetFunction(IdentityList, Invoke.Name);

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
            clone.IntegerDivide = new MetaMethod(IntegerDivide);

            clone.Not = new MetaMethod(Not);

            clone.BitwiseNot = new MetaMethod(BitwiseNot);
            clone.BitwiseAnd = new MetaMethod(BitwiseAnd);
            clone.BitwiseOr = new MetaMethod(BitwiseOr);
            clone.BitwiseXor = new MetaMethod(BitwiseXor);
            clone.LeftShift = new MetaMethod(LeftShift);
            clone.RightShift = new MetaMethod(RightShift);

            clone.Equal = new MetaMethod(Equal);
            clone.Compare = new MetaMethod(Compare);

            clone.At = new MetaMethod(At);
            clone.Set = new MetaMethod(Set);
            clone.Remainder = new MetaMethod(Remainder);
            clone.Count = new MetaMethod(Count);
            clone.Concat = new MetaMethod(Concat);
            clone.Push = new MetaMethod(Push);
            clone.Pop = new MetaMethod(Pop);
            clone.Contains = new MetaMethod(Contains);

            clone.Invoke = new MetaMethod(Invoke);
            clone.InvokeMagic = new MetaMethod(InvokeMagic);
            clone.ApplyMacro = new MetaMethod(ApplyMacro);
            clone.AsString = new MetaMethod(AsString);
            clone.Type = new MetaMethod(Type);

            clone.InitializeDictionary();

            return clone;
        }

        private void InitializeDictionary()
        {
            var metaMethodList = new List<MetaMethod>
            {
                Add, Subtract, Multiply, Divide, Modulus, Power, IntegerDivide,
                Not,
                BitwiseNot, BitwiseAnd, BitwiseOr, BitwiseXor, LeftShift, RightShift,
                Equal, Compare,
                At, Set, Remainder, Count, Concat, Pop, Push, Contains,
                Invoke, InvokeMagic, ApplyMacro,
                AsString, Type
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
        public MetaMethod IntegerDivide { get; private set; }
        
        // Logical
        public MetaMethod Not { get; private set; }

        // Bitwise
        public MetaMethod BitwiseNot { get; private set; }
        public MetaMethod BitwiseAnd { get; private set; }
        public MetaMethod BitwiseOr { get; private set; }
        public MetaMethod BitwiseXor { get; private set; }
        public MetaMethod LeftShift { get; private set; }
        public MetaMethod RightShift { get; private set; }


        // Comparison
        public MetaMethod Equal { get; private set; }
        public MetaMethod Compare { get; private set; }

        // List
        public MetaMethod At { get; private set; }
        public MetaMethod Set { get; private set; }
        public MetaMethod Remainder { get; private set; }
        public MetaMethod Count { get; private set; }
        public MetaMethod Concat { get; private set; }
        public MetaMethod Push { get; private set; }
        public MetaMethod Pop { get; private set; }
        public MetaMethod Contains { get; private set; }

        // Other
        public MetaMethod Invoke { get; private set; }
        public MetaMethod InvokeMagic { get; private set; }
        public MetaMethod ApplyMacro { get; private set; }
        public MetaMethod AsString { get; private set; }
        public MetaMethod Type { get; private set; }

        protected static IValue IdentityEqual (List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            IValue second = arguments.Skip(1).First();
            return first.Equals(second) ? Bool.True : Bool.False;
        }

        protected static IValue IdentityString(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            return new WulString(first.AsString());
        }

        protected static IValue IdentityType(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            return (IValue) first.Type ?? Value.Nil;
        }

        protected static IValue IdentityList(List<IValue> arguments, Scope s)
        {
            return new ListTable(arguments);
        }
    }
}
