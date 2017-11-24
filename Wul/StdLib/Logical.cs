using System.Linq;
using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class Logical
    {
        internal static IFunction Not = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Not.Invoke(list, scope);
        }, "not");
    }
}
