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

        [NetFunction("gc.collect")]
        internal static IValue CollectGarbage(List<IValue> list, Scope s)
        {
            System.GC.Collect();
            return Value.Nil;
        }

        [NetFunction("gc.usage")]
        internal static IValue MemoryUsage(List<IValue> list, Scope s)
        {
            long workingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            return new NetObject(workingSet);
        }

        [NetFunction("gc.usage.mb")]
        internal static IValue MemoryUsageMegabytes(List<IValue> list, Scope s)
        {
            long workingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
            return (Number) workingSet;
        }

        [NetFunction("debug.getinfo")]
        internal static IValue FunctionInfo(List<IValue> list, Scope s)
        {
            var first = list[0];
            string name = "unnamed";
            string type = "Wul";
            string source = "main";
            int line = 1;

            if (first is IFunction ifunc)
            {
                name = ifunc.Name;
                line = ifunc.Line;
            }

            if (first is Function func)
            {
                type = "Wul";
            }
            else if (first is NetFunction net)
            {
                type = "Net";
            }
            else if (first is MagicFunction magic)
            {
                type = "LazyNet";
            }
            else if (first is MacroFunction macro)
            {
                type = "Macro";
            }

            return new NetObject($"\tName: {name}\n\tType: {type}\n\tSource: {source}\n\tLine: {line}");
        }
    }
}
