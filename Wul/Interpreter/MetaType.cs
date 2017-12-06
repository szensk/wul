using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.Interpreter
{
    public class MetaType : IValue
    {
        public static readonly MetaType DefaultMetaType = new MetaType();

        protected MetaType(MetaType parent)
        {
            Parent = parent ?? DefaultMetaType;

            InitializeDictionary();
        }

        private MetaType()
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

            Invoke = new MetaMethod("invoke");
            InvokeMagic = new MetaMethod("@invoke");
            ApplyMacro = new MetaMethod("apply");
            AsString = new MetaMethod("string");
            Type = new MetaMethod("type");

            InitializeDictionary();
        }

        //Surely there is a better way
        public MetaType Clone()
        {
            MetaType clone = new MetaType(Parent)
            {
                Add = new MetaMethod(Add),
                Subtract = new MetaMethod(Subtract),
                Multiply = new MetaMethod(Multiply),
                Divide = new MetaMethod(Divide),
                Modulus = new MetaMethod(Modulus),
                Power = new MetaMethod(Power),
                IntegerDivide = new MetaMethod(IntegerDivide),
                Not = new MetaMethod(Not),
                BitwiseNot = new MetaMethod(BitwiseNot),
                BitwiseAnd = new MetaMethod(BitwiseAnd),
                BitwiseOr = new MetaMethod(BitwiseOr),
                BitwiseXor = new MetaMethod(BitwiseXor),
                LeftShift = new MetaMethod(LeftShift),
                RightShift = new MetaMethod(RightShift),
                Equal = new MetaMethod(Equal),
                Compare = new MetaMethod(Compare),
                At = new MetaMethod(At),
                Set = new MetaMethod(Set),
                Remainder = new MetaMethod(Remainder),
                Count = new MetaMethod(Count),
                Concat = new MetaMethod(Concat),
                Push = new MetaMethod(Push),
                Pop = new MetaMethod(Pop),
                Invoke = new MetaMethod(Invoke),
                InvokeMagic = new MetaMethod(InvokeMagic),
                ApplyMacro = new MetaMethod(ApplyMacro),
                AsString = new MetaMethod(AsString),
                Type = new MetaMethod(Type)
            };

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
                At, Set, Remainder, Count, Concat, Pop, Push,
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
        private MetaMethod AddMetaMethod;

        public MetaMethod Add
        {
            get => AddMetaMethod ?? Parent?.Add;
            private set => AddMetaMethod = value;
        }

        private MetaMethod SubtractMetaMethod;
        public MetaMethod Subtract
        {
            get => SubtractMetaMethod ?? Parent?.Subtract;
            private set => SubtractMetaMethod = value;
        }

        private MetaMethod MultiplyMetaMethod;
        public MetaMethod Multiply
        {
            get => MultiplyMetaMethod ?? Parent?.Multiply;
            private set => MultiplyMetaMethod = value;
        }

        private MetaMethod DivideMetaMethod;
        public MetaMethod Divide
        {
            get => DivideMetaMethod ?? Parent?.Divide;
            private set => DivideMetaMethod = value;
        }

        private MetaMethod ModulusMetaMethod;
        public MetaMethod Modulus
        {
            get => ModulusMetaMethod ?? Parent?.Modulus;
            private set => ModulusMetaMethod = value;
        }

        private MetaMethod PowerMetaMethod;
        public MetaMethod Power
        {
            get => PowerMetaMethod ?? Parent?.Power;
            private set => PowerMetaMethod = value;
        }

        private MetaMethod IntegerDivideMetaMethod;
        public MetaMethod IntegerDivide
        {
            get => IntegerDivideMetaMethod ?? Parent?.IntegerDivide;
            private set => IntegerDivideMetaMethod = value;
        }

        // Logical
        private MetaMethod NotMetaMethod;
        public MetaMethod Not
        {
            get => NotMetaMethod ?? Parent?.Not;
            private set => NotMetaMethod = value;
        }

        // Bitwise
        private MetaMethod BitwiseNotMetaMethod;
        public MetaMethod BitwiseNot
        {
            get => BitwiseNotMetaMethod ?? Parent?.BitwiseNot;
            private set => BitwiseNotMetaMethod = value;
        }

        private MetaMethod BitwiseAndMetaMethod;
        public MetaMethod BitwiseAnd
        {
            get => BitwiseAndMetaMethod ?? Parent?.BitwiseAnd;
            private set => BitwiseAndMetaMethod = value;
        }

        private MetaMethod BitwiseOrMetaMethod;
        public MetaMethod BitwiseOr
        {
            get => BitwiseOrMetaMethod ?? Parent?.BitwiseOr;
            private set => BitwiseOrMetaMethod = value;
        }

        private MetaMethod BitwiseXorMetaMethod;

        public MetaMethod BitwiseXor
        {
            get => BitwiseXorMetaMethod ?? Parent?.BitwiseXor;
            private set => BitwiseXorMetaMethod = value;
        }

        private MetaMethod LeftShiftMetaMethod;
        public MetaMethod LeftShift
        {
            get => LeftShiftMetaMethod ?? Parent?.LeftShift;
            private set => LeftShiftMetaMethod = value;
        }

        private MetaMethod RightShiftMetaMethod;
        public MetaMethod RightShift
        {
            get => RightShiftMetaMethod ?? Parent?.RightShift;
            private set => RightShiftMetaMethod = value;
        }
        
        // Comparison
        private MetaMethod EqualMetaMethod;
        public MetaMethod Equal
        {
            get => EqualMetaMethod ?? Parent?.Equal;
            private set => EqualMetaMethod = value;
        }

        private MetaMethod CompareMetaMethod;
        public MetaMethod Compare
        {
            get => CompareMetaMethod ?? Parent?.Compare;
            private set => CompareMetaMethod = value;
        }

        // List
        private MetaMethod AtMetaMethod;
        public MetaMethod At
        {
            get => AtMetaMethod ?? Parent?.At;
            private set => AtMetaMethod = value;
        }

        private MetaMethod SetMetaMethod;
        public MetaMethod Set
        {
            get => SetMetaMethod ?? Parent?.Set;
            private set => SetMetaMethod = value;
        }

        private MetaMethod RemainderMetaMethod;
        public MetaMethod Remainder
        {
            get => RemainderMetaMethod ?? Parent?.Remainder;
            private set => RemainderMetaMethod = value;
        }

        private MetaMethod CountMetaMethod;
        public MetaMethod Count
        {
            get => CountMetaMethod ?? Parent?.Count;
            private set => CountMetaMethod = value;
        }

        private MetaMethod ConcatMetaMethod;
        public MetaMethod Concat
        {
            get => ConcatMetaMethod ?? Parent?.Concat;
            //get => ConcatMetaMethod.IsDefined
            //    ? ConcatMetaMethod
            //    : Parent?.Concat.IsDefined ?? false
            //        ? Parent.Concat
            //        : null;
            private set => ConcatMetaMethod = value;
        }

        private MetaMethod PushMetaMethod;
        public MetaMethod Push
        {
            get => PushMetaMethod ?? Parent?.Push;
            private set => PushMetaMethod = value;
        }

        private MetaMethod PopMetaMethod;
        public MetaMethod Pop
        {
            get => PopMetaMethod ?? Parent?.Pop;
            private set => PopMetaMethod = value;
        }

        // Other
        private MetaMethod InvokeMetaMethod;
        public MetaMethod Invoke
        {
            get => InvokeMetaMethod ?? Parent?.Invoke;
            private set => InvokeMetaMethod = value;
        }

        //TODO this should be removed
        private MetaMethod InvokeMagicMetaMethod;
        public MetaMethod InvokeMagic
        {
            get => InvokeMagicMetaMethod ?? Parent?.InvokeMagic;
            private set => InvokeMagicMetaMethod = value;
        }

        private MetaMethod ApplyMacroMetaMethod;
        public MetaMethod ApplyMacro
        {
            get => ApplyMacroMetaMethod ?? Parent?.ApplyMacro;
            private set => ApplyMacroMetaMethod = value;
        }

        private MetaMethod AsStringMetaMethod;
        public MetaMethod AsString
        {
            get => AsStringMetaMethod ?? Parent?.AsString;
            private set => AsStringMetaMethod = value;
        }

        private MetaMethod TypeMetaMethod;
        public MetaMethod Type
        {
            get => TypeMetaMethod ?? Parent?.Type;
            private set => TypeMetaMethod = value;
        }

        public MetaType Parent { get; set; }

        public MetaType Metatype
        {
            get => null; 
            set => throw new NotImplementedException();
        }

        WulType IValue.Type => throw new NotImplementedException();

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            throw new NotImplementedException();
        }

        string IValue.AsString()
        {
            return "MetaType";
        }

        public object ToObject()
        {
            throw new NotImplementedException();
        }

        protected static Bool IdentityEqual (List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            IValue second = arguments.Skip(1).First();
            return first.Equals(second) ? Bool.True : Bool.False;
        }

        protected static UString IdentityString(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            return new UString(first.AsString());
        }

        protected static IValue IdentityType(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();
            return (IValue) first.Type ?? Value.Nil;
        }
    }
}
