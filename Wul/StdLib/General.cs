using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;

namespace Wul.StdLib
{
    class General
    {
        internal static IFunction Let = new NetFunction((list, scope) =>
        {
            var name = list.First() as UString;
            var value = list.Skip(1).Last();
            scope[name.Value] = value;
            return value;
        }, "let", new List<string>());
    }
}
