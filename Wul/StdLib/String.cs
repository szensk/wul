﻿using System.Linq;
using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class String
    {
        internal static IFunction Concat = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Concat.Invoke(list, scope);
        }, "..");

        internal static IFunction Substring = new NetFunction((list, scope) =>
        {
            string value = (list[0] as UString).Value;
            Number start = list[1] as Number;
            Number length = list.Skip(2).FirstOrDefault() as Number;

            string result = "";
            result = length != null ? value.Substring(start, length) : value.Substring(start);
            return new UString(result);
        }, "substring");

        internal static IFunction Lower = new NetFunction((list, scope) =>
        {
            string value = (list[0] as UString).Value;
            return new UString(value.ToLower());
        }, "lower");

        internal static IFunction Upper = new NetFunction((list, scope) =>
        {
            string value = (list[0] as UString).Value;
            return new UString(value.ToUpper());
        }, "upper");
    }
}
