using System.Linq;
using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class Logical
    {
        internal static IFunction Not = new NetFunction((list, scope) =>
        {
            var bools = list.OfType<Bool>();

            var notBools = bools.Select(b => b.Value ? Bool.False : Bool.True);

            if (notBools.Count() <= 1)
            {
                return (IValue) notBools.FirstOrDefault() ?? Value.Nil;
            }
            else
            {
                return new ListTable(notBools);
            }
        }, "not");
    }
}
