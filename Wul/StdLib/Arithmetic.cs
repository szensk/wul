using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Arithmetic
    {
        [NetFunction("inc")]
        internal static IValue Increment(List<IValue> list, Scope scope)
        {
            return Add(list.PushBack((Number) 1), scope);
        }

        [NetFunction("+")]
        internal static IValue Add(List<IValue> list, Scope scope)
        {
            IValue first = list.First().AssertNotNil();
            return first.MetaType.Add.Invoke(list, scope).First();
        }

        [NetFunction("dec")]
        internal static IValue Decrement(List<IValue> list, Scope scope)
        {
            return Subtract(list.PushBack((Number)1), scope);
        }

        [NetFunction("-")]
        internal static IValue Subtract(List<IValue> list, Scope scope)
        {
            IValue first = list.First().AssertNotNil();

            return first.MetaType.Subtract.Invoke(list, scope).First();
        }

        [NetFunction("*")]
        internal static IValue Multiply(List<IValue> list, Scope scope)
        {
            IValue first = list.First().AssertNotNil();

            return first.MetaType.Multiply.Invoke(list, scope).First();
        }

        [NetFunction("/")]
        internal static IValue Divide(List<IValue> list, Scope scope)
        {
            IValue first = list.First().AssertNotNil();

            return first.MetaType.Divide.Invoke(list, scope).First();
        }

        [NetFunction("//")]
        internal static IValue IntegerDivide(List<IValue> list, Scope scope)
        {
            IValue first = list.First().AssertNotNil();

            return first.MetaType.IntegerDivide.Invoke(list, scope).First();
        }

        [NetFunction("%")]
        internal static IValue Modulus(List<IValue> list, Scope scope)
        {
            IValue first = list.First().AssertNotNil();

            return first.MetaType.Modulus.Invoke(list, scope).First();
        }

        [NetFunction("**")]
        internal static IValue Power(List<IValue> list, Scope scope)
        {
            IValue first = list.First().AssertNotNil();

            return first.MetaType.Power.Invoke(list, scope).First();
        }

        [NetFunction("int")]
        internal static IValue NetInteger(List<IValue> list, Scope scope)
        {
            if (list.Count == 1)
            {
                Number first = (Number) list[0].AssertNotNil();
                return new NetObject((int)first.Value);
            }
            else if (list.Count > 1)
            {
                int[] intArray = list.Select(n => (int) ((Number) n.AssertNotNil()).Value).ToArray();
                return new NetObject(intArray);
            }
            else
            {
                return Value.Nil;
            }
        }
    }
}
