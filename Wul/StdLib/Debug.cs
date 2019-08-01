using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.StdLib.Attribute;

namespace Wul.StdLib
{
    [StdLib]
    internal class Debug
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
            string type = "unknown";
            string source = "";
            int line = 0;

            if (first is IFunction ifunc)
            {
                name = ifunc.Name;
                line = ifunc.Line;
                source = ifunc.FileName;
                type = ifunc.GetType().Name;
            }
            else if (first is WulType wt)
            {
                name = wt.Name;
                line = wt.Line;
                source = wt.FileName;
                type = "Type";
            }

            return MapTable.FromObject(new
            {
                Name = (WulString) name,
                Type = (WulString) type,
                Source = (WulString) source,
                Line = (Number) line
            });
        }

        [NetFunction("debug.name")]
        internal static IValue GetNameOfValue(List<IValue> list, Scope s)
        {
            var first = list[0];
            if (first is IFunction func)
            {
                return new WulString(func.Name);
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
            var map = s.BoundVariables.ToDictionary(k => (IValue) new WulString(k.Key), v => v.Value.Value);
            return new MapTable(map);
        }

        [NetFunction("debug.trace")]
        internal static IValue Traceback(List<IValue> list, Scope s)
        {
            foreach (var f in WulInterpreter.CallStack.Skip(s == null ? 0 : 1))
            {
                Console.WriteLine(f);
            }
            return Value.Nil;
        }

        [NetFunction("debug.upvals")]
        internal static IValue Upvals(List<IValue> list, Scope s)
        {
            var first = list[0];
            if (first is Function func)
            {
                var upvals = func.ParentScope.BoundVariables
                    .ToDictionary(
                        k => (IValue) new WulString(k.Key), 
                        v => v.Value.Value
                    );
                return new MapTable(upvals);
            }
            return Value.Nil;
        }
    }
}
