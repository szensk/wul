using System.Collections.Generic;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    class Debug
    {
        [NetFunction("sentinel")]
        internal static IValue NewSentinel(List<IValue> list, Scope s)
        {
            return new Sentinel();
        }
    }
}
