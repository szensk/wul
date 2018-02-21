using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class NumberMetaType : MetaType
    {
        public static readonly NumberMetaType Instance = new NumberMetaType();

        private NumberMetaType()
        {
            //Arithmetic
            Add.Method = NetFunction.FromSingle(DoAdd, Add.Name);
            Subtract.Method = NetFunction.FromSingle(DoSubtract, Subtract.Name);
            Multiply.Method = NetFunction.FromSingle(DoMultiply, Multiply.Name);
            Divide.Method = NetFunction.FromSingle(DoDivide, Divide.Name);
            Modulus.Method = NetFunction.FromSingle(DoModulus, Modulus.Name);
            Power.Method = NetFunction.FromSingle(DoPower, Power.Name);
            IntegerDivide.Method = NetFunction.FromSingle(DoIntegerDivide, IntegerDivide.Name);

            //Bitwise
            BitwiseNot.Method = NetFunction.FromSingle(NumberBitwiseNot, BitwiseNot.Name);
            BitwiseAnd.Method = NetFunction.FromSingle(NumberBitwiseAnd, BitwiseAnd.Name);
            BitwiseOr.Method = NetFunction.FromSingle(NumberBitwiseOr, BitwiseOr.Name);
            BitwiseXor.Method = NetFunction.FromSingle(NumberBitwiseXor, BitwiseXor.Name);
            LeftShift.Method = NetFunction.FromSingle(NumberLeftShift, LeftShift.Name);
            RightShift.Method = NetFunction.FromSingle(NumberRightShift, RightShift.Name);

            //Comparison
            Equal.Method = NetFunction.FromSingle(AreEqual, Equal.Name);
            Compare.Method = NetFunction.FromSingle(Comparison, Compare.Name);

            //Other
            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(IdentityType, Type.Name);
        }

        private IValue DoAdd(List<IValue> arguments, Scope s)
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

        private IValue DoSubtract(List<IValue> arguments, Scope s)
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

        private IValue DoMultiply(List<IValue> arguments, Scope s)
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

        private IValue DoDivide(List<IValue> arguments, Scope s)
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

        private IValue DoIntegerDivide(List<IValue> arguments, Scope s)
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

        private IValue DoModulus(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)(first.Value % second.Value);
        }

        private IValue DoPower(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)Math.Pow(first.Value, second.Value);
        }

        private static Number NumberBitwiseAnd(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first & second;
            return result;
        }

        private static Number NumberBitwiseOr(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first | second;
            return result;
        }

        private static Number NumberBitwiseXor(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first ^ second;
            return result;
        }

        private static Number NumberLeftShift(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first << (int)second;
            return result;
        }

        private static Number NumberRightShift(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            int result = first >> (int)second;
            return result;
        }

        private static Number NumberBitwiseNot(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 1) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;

            if (first == null) throw new Exception("Argument not a number");

            return ~first;
        }

        private IValue AreEqual(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return Equals(first, second) ? Bool.True : Bool.False;
        }

        private IValue Comparison(List<IValue> arguments, Scope s)
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