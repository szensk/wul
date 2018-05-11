using System;
using System.Collections.Generic;
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
            public IEnumerable<MultiMagicFunctionAttribute> MultiMagicAttributes;
        }

        private static string GetFileName(IWulFunction funcDesc)
        {
            return $"{System.IO.Path.GetFileName(funcDesc.FileName)} {funcDesc.Member}";
        }

        //TODO replace with a generic method
        private static void RegisterNetFunction(FunctionRegistration method)
        {
            var first = method.NetAttributes.First();
            string defaultName = first.Name;
            var deleg = method.Method.CreateDelegate(typeof(Func<List<IValue>, Scope, IValue>));
            NetFunction netFunction = new NetFunction((Func<List<IValue>, Scope, IValue>)deleg, defaultName, first.Line, GetFileName(first));
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
            NetFunction netFunction = new MultiNetFunction((Func<List<IValue>, Scope, List <IValue>>)deleg, defaultName, first.Line, GetFileName(first));
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
            MagicFunction magicFunction = new MagicFunction((Func<ListNode, Scope, IValue>)deleg, defaultName, first.Line, GetFileName(first));
            foreach (var globalname in method.MagicAttributes)
            {
                Scope[globalname.Name] = magicFunction;
            }
        }

        private static void RegisterMultiMagicFunction(FunctionRegistration method)
        {
            var first = method.MultiMagicAttributes.First();
            string defaultName = first.Name;
            var deleg = method.Method.CreateDelegate(typeof(Func<ListNode, Scope, List<IValue>>));
            MagicFunction magicFunction = new MultiMagicFunction((Func<ListNode, Scope, List<IValue>>)deleg, defaultName, first.Line, GetFileName(first));
            foreach (var globalname in method.MultiMagicAttributes)
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

            //Bools
            Scope["true"] = Bool.True;
            Scope["false"] = Bool.False;

            var types = Assembly.GetAssembly(typeof(Global)).GetTypes();

            var namedMethods = types.SelectMany(t => t.GetRuntimeMethods())
                .Select(m => new FunctionRegistration
                {
                    Method = m,
                    NetAttributes = m.GetCustomAttributes<NetFunctionAttribute>(),
                    MultiNetAttributes = m.GetCustomAttributes<MultiNetFunctionAttribute>(),
                    MagicAttributes = m.GetCustomAttributes<MagicFunctionAttribute>(),
                    MultiMagicAttributes = m.GetCustomAttributes<MultiMagicFunctionAttribute>()
                })
                .ToList();

            foreach (FunctionRegistration method in namedMethods)
            {
                if (method.MultiNetAttributes.Any())
                {
                    RegisterMultiNetFunction(method);
                }
                else if (method.MultiMagicAttributes.Any())
                {
                    RegisterMultiMagicFunction(method);
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
        }
    }
}
