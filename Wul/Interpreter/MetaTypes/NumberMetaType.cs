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
            Add.Method = new NetFunction(DoAdd, Add.Name);
            Subtract.Method = new NetFunction(DoSubtract, Subtract.Name);
            Multiply.Method = new NetFunction(DoMultiply, Multiply.Name);
            Divide.Method = new NetFunction(DoDivide, Divide.Name);
            Modulus.Method = new NetFunction(DoModulus, Modulus.Name);
            Power.Method = new NetFunction(DoPower, Power.Name);

            //TODO Bitwise and maybe some logical

            //Comparison
            Equal.Method = new NetFunction(AreEqual, Equal.Name);
            Compare.Method = new NetFunction(Comparison, Compare.Name);

            //Other
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
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

        public IValue DoModulus(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)((int)first.Value % (int)second.Value);
        }

        public IValue DoPower(List<IValue> arguments, Scope s)
        {
            var numbers = arguments.Select(x => x as Number).ToArray();
            var first = numbers.First();
            var second = numbers.Skip(1).First();
            return (Number)Math.Pow(first.Value, second.Value);
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