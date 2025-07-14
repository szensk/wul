using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.StdLib.Attribute;

namespace Wul.StdLib
{
    [StdLib]
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

        //TODO very similar to dump
        [MultiMagicFunction("@string")]
        internal static List<IValue> MetaStringify(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();
            if (children.Length == 1)
            {
                IValue first = children.First();
                var str = new WulString(first.ToString());
                return Value.ListWith(str);
            }
            else if (children.Length > 1)
            {
                var results = children.Select(c => new WulString(c.ToString())).Cast<IValue>();
                return Value.ListWith(results.ToArray());
            }
            throw new Exception("no arguments supplied to string");
        }

        [NetFunction("substring")]
        internal static IValue Substring(List<IValue> list, Scope scope)
        {
            string value = ((WulString)list[0]).Value;
            Number start = list[1] as Number;
            Number length = list.Skip(2).FirstOrDefault() as Number;

            var result = length != null ? value.Substring(start, length) : value.Substring(start);
            return new WulString(result);
        }

        [NetFunction("lower")]
        internal static IValue Lower(List<IValue> list, Scope scope)
        {
            string value = ((WulString)list[0]).Value;
            return new WulString(value.ToLower());
        }

        [NetFunction("upper")]
        internal static IValue Upper(List<IValue> list, Scope scope)
        {
            string value = ((WulString)list[0]).Value;
            return new WulString(value.ToUpper());
        }

        [NetFunction("split")]
        internal static IValue Split(List<IValue> list, Scope scope)
        {
            string value = ((WulString)list[0]).Value;
            string splitOn = ((WulString)list[1]).Value;
            var results = value.Split(splitOn);
            return new ListTable(results.Select(n => (WulString)n));

        }

        [MultiNetFunction("number")]
        internal static List<IValue> Numberify(List<IValue> list, Scope scope)
        {
            if (list.Count == 1)
            {
                IValue first = list.First();
                return Value.ListWith(Helpers.ToNumber(first));
            }
            else if (list.Count > 1)
            {
                var results = list.Select(Helpers.ToNumber).Cast<IValue>();
                return Value.ListWith(results.ToArray());
            }
            throw new Exception("no arguments supplied to string");
        }
    }
}
