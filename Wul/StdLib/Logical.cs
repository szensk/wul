using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Logical
    {
        [GlobalName("not")]
        internal static IFunction Not = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Not.Invoke(list, scope);
        }, "not");

        [GlobalName("or")]
        internal static IFunction Or = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Or.Invoke(list, scope);
        }, "or");

        [GlobalName("and")]
        internal static IFunction And = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.And.Invoke(list, scope);
        }, "and");

        [GlobalName("xor")]
        internal static IFunction Xor = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Xor.Invoke(list, scope);
        }, "or");
    }
}
