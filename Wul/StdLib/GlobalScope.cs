using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class Global
    {
        private static Scope _scope;

        public static Scope Scope => _scope ?? (_scope = new Scope(null));

        private class FunctionRegistration
        {
            public MethodInfo Method;
            public IEnumerable<NetFunctionAttribute> NetAttributes;
            public IEnumerable<MagicNetFunctionAttribute> MagicAttributes;
        }

        private static void RegisterNetFunction(FunctionRegistration method)
        {
            string defaultName = method.NetAttributes.First().Name;
            var deleg = method.Method.CreateDelegate(typeof(Func<List<IValue>, Scope, IValue>));
            NetFunction netFunction = new NetFunction((Func<List<IValue>, Scope, IValue>)deleg, defaultName);
            foreach (var globalname in method.NetAttributes)
            {
                Scope[globalname.Name] = netFunction;
            }
        }

        private static void RegisterMagicFunction(FunctionRegistration method)
        {
            string defaultName = method.MagicAttributes.First().Name;
            var deleg = method.Method.CreateDelegate(typeof(Func<ListNode, Scope, IValue>));
            MagicNetFunction magicFunction = new MagicNetFunction((Func<ListNode, Scope, IValue>)deleg, defaultName);
            foreach (var globalname in method.MagicAttributes)
            {
                Scope[globalname.Name] = magicFunction;
            }
        }

        public static void RegisterDefaultFunctions()
        {
            //Types
            Scope["Bool"] = BoolType.Instance;
            Scope["Number"] = NumberType.Instance;
            Scope["List"] = ListType.Instance;
            Scope["Map"] = MapType.Instance;
            Scope["String"] = StringType.Instance;
            Scope["Function"] = FunctionType.Instance;
            Scope["SyntaxNode"] = SyntaxNodeType.Instance;
            Scope["Range"] = RangeType.Instance;

            var types = Assembly.GetAssembly(typeof(Global)).GetTypes();

            var namedMethods = types.SelectMany(t => t.GetRuntimeMethods())
                .Select(m => new FunctionRegistration
                {
                    Method = m,
                    NetAttributes = m.GetCustomAttributes<NetFunctionAttribute>(),
                    MagicAttributes = m.GetCustomAttributes<MagicNetFunctionAttribute>()
                })
                .ToList();

            foreach (FunctionRegistration method in namedMethods)
            {
                if (method.NetAttributes.Any())
                {
                    RegisterNetFunction(method);
                }
                else if (method.MagicAttributes.Any())
                {
                    RegisterMagicFunction(method);
                }
            }

            FunctionMetaType.SetMetaMethods();
        }
    }
}
