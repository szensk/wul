using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class String
    {
        [NetFunction("..")]
        [NetFunction("concat")]
        internal static IValue Concat(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Concat.Invoke(list, scope);
        }

        [NetFunction("string")]
        internal static IValue Stringify(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.AsString.Invoke(list, scope);
        }

        [NetFunction("substring")]
        internal static IValue Substring(List<IValue> list, Scope scope)
        {
            string value = ((UString) list[0]).Value;
            Number start = list[1] as Number;
            Number length = list.Skip(2).FirstOrDefault() as Number;

            string result = "";
            result = length != null ? value.Substring(start, length) : value.Substring(start);
            return new UString(result);
        }

        [NetFunction("lower")]
        internal static IValue Lower(List<IValue> list, Scope scope)
        {
            string value = ((UString) list[0]).Value;
            return new UString(value.ToLower());
        }

        [NetFunction("upper")]
        internal static IValue Upper(List<IValue> list, Scope scope)
        {
            string value = ((UString) list[0]).Value;
            return new UString(value.ToUpper());
        }
    }
}
