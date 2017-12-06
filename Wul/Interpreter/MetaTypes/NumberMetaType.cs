using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class NumberMetaType : MetaType
    {
        public static readonly NumberMetaType Instance = new NumberMetaType();

        private NumberMetaType() : base(null)
        {
            //Arithmetic
            Add = new NetFunction(DoAdd, Add.Name);
            Subtract = new NetFunction(DoSubtract, Subtract.Name);
            Multiply = new NetFunction(DoMultiply, Multiply.Name);
            Divide = new NetFunction(DoDivide, Divide.Name);
            Modulus = new NetFunction(DoModulus, Modulus.Name);
            Power = new NetFunction(DoPower, Power.Name);
            IntegerDivide = new NetFunction(DoIntegerDivide, IntegerDivide.Name);

            //Bitwise
            BitwiseNot = new NetFunction(NumberBitwiseNot, BitwiseNot.Name);
            BitwiseAnd = new NetFunction(NumberBitwiseAnd, BitwiseAnd.Name);
            BitwiseOr = new NetFunction(NumberBitwiseOr, BitwiseOr.Name);
            BitwiseXor = new NetFunction(NumberBitwiseXor, BitwiseXor.Name);
            LeftShift = new NetFunction(NumberLeftShift, LeftShift.Name);
            RightShift = new NetFunction(NumberRightShift, RightShift.Name);

            //Comparison
            Equal = new NetFunction(AreEqual, Equal.Name);
            Compare = new NetFunction(Comparison, Compare.Name);

            //Other
            AsString = new NetFunction(IdentityString, AsString.Name);
            Type = new NetFunction(IdentityType, Type.Name);

            InitializeDictionary();
        }

        public IValue DoAdd(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(a => a as Number).ToList();
            if (!numbers.Any())
            {
                return Value.Nil;
            }
            if (numbers.Any(a => a == null))
            {
                throw new InvalidOperationException("All arguments to + must be numbers");
            }
            double sum = numbers.Sum(x => x.Value);
            return (Number)sum;
        }

        public IValue DoSubtract(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 1)
            {
                return Value.Nil;
            }
            var numbers = arguments.Select(a => a as Number).ToList();
            if (numbers.Any(a => a == null))
            {
                throw new InvalidOperationException("All arguments to - must be numbers");
            }

            Number first;
            double sum;
            if (arguments.Count < 2)
            {
                first = 0;
                sum = ((Number)arguments.First()).Value;
            }
            else
            {
                first = numbers.First();
                sum = numbers.Skip(1).Sum(x => x.Value);
            }
            return (Number)(first.Value - sum);
        }

        public IValue DoMultiply(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            if (!numbers.Any())
            {
                return Value.Nil;
            }
            if (numbers.Any(a => a == null))
            {
                throw new InvalidOperationException("All arguments to * must be numbers");
            }

            double multiplied = numbers[0].Value;
            for (int i = 1; i < numbers.Length; ++i)
            {
                multiplied *= numbers[i].Value;
            }
            return (Number)multiplied;
        }

        public IValue DoDivide(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            if (!numbers.Any())
            {
                return Value.Nil;
            }
            if (numbers.Any(a => a == null))
            {
                throw new InvalidOperationException("All arguments to / must be numbers");
            }

            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)(first.Value / second.Value);
        }

        public IValue DoIntegerDivide(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            if (!numbers.Any())
            {
                return Value.Nil;
            }
            if (numbers.Any(a => a == null))
            {
                throw new InvalidOperationException("All arguments to / must be numbers");
            }

            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)Math.Floor(first.Value / second.Value);
        }

        public IValue DoModulus(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)(first.Value % second.Value);
        }

        public IValue DoPower(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)Math.Pow(first.Value, second.Value);
        }

        internal static Number NumberBitwiseAnd(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first & second;
            return result;
        }

        internal static Number NumberBitwiseOr(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first | second;
            return result;
        }

        internal static Number NumberBitwiseXor(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first ^ second;
            return result;
        }

        internal static Number NumberLeftShift(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first << (int)second;
            return result;
        }

        internal static Number NumberRightShift(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first >> (int)second;
            return result;
        }

        internal static Number NumberBitwiseNot(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 1) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;

            if (first == null) throw new Exception("Argument not a number");

            return ~first;
        }

        public IValue AreEqual(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return Equals(first, second) ? Bool.True : Bool.False;
        }

        public IValue Comparison(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            Number result = 0;
            if (first.Value < second.Value) result = -1;
            if (first.Value > second.Value) result = 1;
            return result;
        }
    }
}