using System;
using System.Collections.Generic;
using System.Linq;
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
                source = func.FileName;
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
                source = macro.FileName;
            }

            return MapTable.FromObject(new
            {
                Name = (UString) name,
                Type = (UString) type,
                Source = (UString) source,
                Line = (Number) line
            });
        }

        [NetFunction("debug.name")]
        internal static IValue GetNameOfValue(List<IValue> list, Scope s)
        {
            var first = list[0];
            if (first is IFunction func)
            {
                return new UString(func.Name);
            }
            return Value.Nil;
        }

        [NetFunction("debug.scope")]
        internal static IValue ScopeToMap(List<IValue> list, Scope s)
        {
            var first = list.FirstOrDefault();
            if (first != null)
            {
                int level = (Number) first;
                while (s.Parent != null && level > 0)
                {
                    level--;
                    s = s.Parent;
                }
                if (level > 0) { throw new Exception("Invalid scope level: not enough parent scopes");}
            }
            var map = s.BoundVariables.ToDictionary(k => (IValue) new UString(k.Key), v => v.Value.Value);
            return new MapTable(map);
        }
    }
}
