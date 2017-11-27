using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class String
    {
        [GlobalName("..")]
        [GlobalName("concat")]
        internal static IFunction Concat = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Concat.Invoke(list, scope);
        }, "..");

        [GlobalName("string")]
        internal static IFunction Stringify = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.AsString.Invoke(list, scope);
        }, "string");

        [GlobalName("substring")]
        internal static IFunction Substring = new NetFunction((list, scope) =>
        {
            string value = ((UString) list[0]).Value;
            Number start = list[1] as Number;
            Number length = list.Skip(2).FirstOrDefault() as Number;

            string result = "";
            result = length != null ? value.Substring(start, length) : value.Substring(start);
            return new UString(result);
        }, "substring");

        [GlobalName("lower")]
        internal static IFunction Lower = new NetFunction((list, scope) =>
        {
            string value = ((UString) list[0]).Value;
            return new UString(value.ToLower());
        }, "lower");

        [GlobalName("upper")]
        internal static IFunction Upper = new NetFunction((list, scope) =>
        {
            string value = ((UString) list[0]).Value;
            return new UString(value.ToUpper());
        }, "upper");
    }
}
