using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    class Bitwise
    {
        [NetFunction("&")]
        internal static IValue BitwiseAnd(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();

            return first.MetaType.BitwiseAnd.Invoke(arguments, s).First();
        }

        [NetFunction("|")]
        internal static IValue BitwiseOr(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();

            return first.MetaType.BitwiseOr.Invoke(arguments, s).First();
        }

        [NetFunction("^")]
        internal static IValue BitwiseXor(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();

            return first.MetaType.BitwiseXor.Invoke(arguments, s).First();
        }

        [NetFunction("<<")]
        internal static IValue LeftShift(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();

            return first.MetaType.LeftShift.Invoke(arguments, s).First();
        }

        [NetFunction(">>")]
        internal static IValue RightShift(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();

            return first.MetaType.RightShift.Invoke(arguments, s).First();
        }

        [NetFunction("~")]
        internal static IValue BitwiseNot(List<IValue> arguments, Scope s)
        {
            IValue first = arguments.First();

            return first.MetaType.BitwiseNot.Invoke(arguments, s).First();
        }

        [NetFunction("bin")]
        internal static WulString ToBinary(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 1) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;

            if (first == null) throw new Exception("Argument not a number");
            var binaryString = Convert.ToString(first, 2);
            return new WulString(binaryString);
        }

        [NetFunction("hex")]
        internal static WulString ToHexadecimal(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 1) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;

            if (first == null) throw new Exception("Argument not a number");
            var binaryString = Convert.ToString(first, 16);
            return new WulString(binaryString);
        }

        [NetFunction("base")]
        internal static WulString ToBase(List<IValue> arguments, Scope s)
        {
            if (arguments.Count < 2) throw new Exception("Invalid number of arguments &");
            Number first = arguments[0] as Number;
            Number second = arguments[1] as Number;

            if (first == null || second == null) throw new Exception("Argument not a number");

            var binaryString = Convert.ToString(first, second);
            return new WulString(binaryString);
        }
    }
}
