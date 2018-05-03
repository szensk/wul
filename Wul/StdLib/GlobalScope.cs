using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.StdLib
{
    class Global
    {
        private static Scope _scope;

        public static Scope Scope => _scope ?? (_scope = new Scope());

        private class FunctionRegistration
        {
            public MethodInfo Method;
            public IEnumerable<NetFunctionAttribute> NetAttributes;
            public IEnumerable<MultiNetFunctionAttribute> MultiNetAttributes;
            public IEnumerable<MagicFunctionAttribute> MagicAttributes;
        }

        private static string GetFileName(IWulFunction funcDesc)
        {
            return $"{System.IO.Path.GetFileName(funcDesc.FileName)} {funcDesc.Member}";
        }

        private static void RegisterNetFunction(FunctionRegistration method)
        {
            var first = method.NetAttributes.First();
            string defaultName = first.Name;
            var deleg = method.Method.CreateDelegate(typeof(Func<List<IValue>, Scope, IValue>));
            NetFunction netFunction = NetFunction.FromSingle((Func<List<IValue>, Scope, IValue>)deleg, defaultName, first.Line, GetFileName(first));
            foreach (var globalname in method.NetAttributes)
            {
                Scope[globalname.Name] = netFunction;
            }
        }

        private static void RegisterMultiNetFunction(FunctionRegistration method)
        {
            var first = method.MultiNetAttributes.First();
            string defaultName = first.Name;
            var deleg = method.Method.CreateDelegate(typeof(Func<List<IValue>, Scope, List<IValue>>));
            NetFunction netFunction = new NetFunction((Func<List<IValue>, Scope, List <IValue>>)deleg, defaultName, first.Line, GetFileName(first));
            foreach (var globalname in method.MultiNetAttributes)
            {
                Scope[globalname.Name] = netFunction;
            }
        }

        private static void RegisterMagicFunction(FunctionRegistration method)
        {
            var first = method.MagicAttributes.First();
            string defaultName = first.Name;
            var deleg = method.Method.CreateDelegate(typeof(Func<ListNode, Scope, IValue>));
            MagicFunction magicFunction = MagicFunction.FromSingle((Func<ListNode, Scope, IValue>)deleg, defaultName, first.Line, GetFileName(first));
            foreach (var globalname in method.MagicAttributes)
            {
                Scope[globalname.Name] = magicFunction;
            }
        }

        public static void RegisterDefaultFunctions()
        {
            Stopwatch sw = Stopwatch.StartNew();
            
            //Types
            Scope["Bool"] = BoolType.Instance;
            Scope["Number"] = NumberType.Instance;
            Scope["List"] = ListType.Instance;
            Scope["Map"] = MapType.Instance;
            Scope["String"] = StringType.Instance;
            Scope["Function"] = FunctionType.Instance;
            Scope["SyntaxNode"] = SyntaxNodeType.Instance;
            Scope["Range"] = RangeType.Instance;

            //Bools
            Scope["true"] = Bool.True;
            Scope["false"] = Bool.False;
            Scope["debug.callstack"] = Debug.Callstack;

            var types = Assembly.GetAssembly(typeof(Global)).GetTypes();

            var namedMethods = types.SelectMany(t => t.GetRuntimeMethods())
                .Select(m => new FunctionRegistration
                {
                    Method = m,
                    NetAttributes = m.GetCustomAttributes<NetFunctionAttribute>(),
                    MultiNetAttributes = m.GetCustomAttributes<MultiNetFunctionAttribute>(),
                    MagicAttributes = m.GetCustomAttributes<MagicFunctionAttribute>(),
                })
                .ToList();

            foreach (FunctionRegistration method in namedMethods)
            {
                if (method.MultiNetAttributes.Any())
                {
                    RegisterMultiNetFunction(method);
                }
                else if (method.MagicAttributes.Any())
                {
                    RegisterMagicFunction(method);
                }
                else if (method.NetAttributes.Any())
                {
                    RegisterNetFunction(method);
                }
            }

            FunctionMetaType.SetMetaMethods();

            sw.Stop();
            System.Diagnostics.Debug.WriteLine($"Registration: {sw.Elapsed.TotalSeconds}s");
        }
    }
}
