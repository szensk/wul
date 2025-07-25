﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.StdLib.Attribute;

namespace Wul.StdLib
{
    class Global
    {
        private static Scope _scope;

        public static Scope Scope => _scope ??= new Scope();

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
            var firstParameterType = method.Method.GetParameters()[0].ParameterType;
            NetFunction netFunction;
            if (typeof(IValue).IsAssignableFrom(firstParameterType))
            {
                var deleg = method.Method.CreateDelegate<Func<IValue, Scope, IValue>>();
                netFunction = new NetFunction(deleg, defaultName, first.Line, GetFileName(first));
            }
            else
            {
                var deleg = method.Method.CreateDelegate<Func<List<IValue>, Scope, IValue>>();
                netFunction = new NetFunction(deleg, defaultName, first.Line, GetFileName(first));
            }
            
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

        public static void LoadWulStandardLibrary(Scope s)
        {
            var assembly = Assembly.GetExecutingAssembly(); 
            foreach (var resourceName in assembly.GetManifestResourceNames().Where(s => s.EndsWith(".wul")))
            {
                LoadWulStandardLibraryResource(assembly, resourceName, s);
            }
        }

        public static void LoadWulStandardLibraryResource(Assembly assembly, string resourceName, Scope s)
        {
            try
            {
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (stream == null) return;

                // Read the resource content
                using StreamReader reader = new(stream);
                string content = reader.ReadToEnd();
                Helpers.LoadString(content, s);
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to load resource");
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public static void RegisterDefaultFunctions(Scope s = null)
        {
            s ??= Scope;

            //Types
            s["Bool"] = BoolType.Instance;
            s["Number"] = NumberType.Instance;
            s["List"] = ListType.Instance;
            s["Map"] = MapType.Instance;
            s["String"] = StringType.Instance;
            s["Function"] = FunctionType.Instance;
            s["SyntaxNode"] = SyntaxNodeType.Instance;
            s["Range"] = RangeType.Instance;
            FunctionMetaType.SetMetaMethods();

            //Bools
            s["true"] = Bool.True;
            s["false"] = Bool.False;

            //Version
            s["wul.version"] = (WulString) Wul.Version;

            var types = Assembly.GetAssembly(typeof(Global)).GetTypes();

            var namedMethods = types
                .Where(t => t.GetCustomAttribute<StdLibAttribute>() != null)
                .SelectMany(t => t.GetRuntimeMethods())
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

            LoadWulStandardLibrary(s);
        }
    }
}
