using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class String
    {
        [NetFunction("concat")]
        [NetFunction("..")]
        internal static IValue Concat(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Concat.Invoke(list, scope).First();
        }

        [MultiNetFunction("string")]
        internal static List<IValue> Stringify(List<IValue> list, Scope scope)
        {
            if (list.Count == 1)
            {
                IValue first = list.First();
                return Value.ListWith(Helpers.ToWulString(first));
            }
            else if (list.Count > 1)
            {
                var results = list.Select(Helpers.ToWulString).Cast<IValue>();
                return Value.ListWith(results.ToArray());
            }
            throw new Exception("no arguments supplied to string");
        }

        [NetFunction("substring")]
        internal static IValue Substring(List<IValue> list, Scope scope)
        {
            string value = ((WulString) list[0]).Value;
            Number start = list[1] as Number;
            Number length = list.Skip(2).FirstOrDefault() as Number;

            var result = length != null ? value.Substring(start, length) : value.Substring(start);
            return new WulString(result);
        }

        [NetFunction("lower")]
        internal static IValue Lower(List<IValue> list, Scope scope)
        {
            string value = ((WulString) list[0]).Value;
            return new WulString(value.ToLower());
        }

        [NetFunction("upper")]
        internal static IValue Upper(List<IValue> list, Scope scope)
        {
            string value = ((WulString) list[0]).Value;
            return new WulString(value.ToUpper());
        }
    }
}
