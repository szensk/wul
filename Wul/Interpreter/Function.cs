﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Parser;

namespace Wul.Interpreter
{
    class Function : IFunction
    {
        public ListNode Body;

        public Function(ListNode body, string name, List<string> argumentNames)
        {
            Name = name;
            Body = body;
            ArgumentNames = argumentNames;
        }

        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public IValue Evaluate(List<IValue> arguments, Scope scope)
        {
            Scope currentScope = scope.EmptyChildScope();

            //Bind arguments to names
            for (int i = 0; i < ArgumentNames.Count; ++i)
            {
                string argName = ArgumentNames[i];
                IValue argValue = i >= arguments.Count ? Value.Nil : arguments[i];
                currentScope[argName] = argValue;
            }

            IValue result = WulInterpreter.Interpret(Body, currentScope);
            return result;
        }

        public virtual IValue Execute(ListNode list, Scope scope)
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }

    class MagicFunction : IFunction
    {
        public ListNode Body;

        public MagicFunction(ListNode body, string name, List<string> argumentNames)
        {
            Name = name;
            Body = body;
            ArgumentNames = argumentNames;
        }

        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public IValue Evaluate(List<IValue> arguments, Scope scope)
        {
            throw new NotImplementedException();
        }

        public virtual IValue Execute(ListNode list, Scope scope)
        {
            Scope currentScope = scope.EmptyChildScope();

            var arguments = list.Children.Skip(1).ToArray();

            //Bind arguments to names
            for (int i = 0; i < arguments.Length; ++i)
            {
                string argName = ArgumentNames[i];
                currentScope[argName] = arguments[i];
            }

            IValue result = WulInterpreter.Interpret(Body, currentScope);
            return result;
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }

    class NetFunction : IFunction
    {
        private readonly Func<List<IValue>, Scope, IValue> Body;

        public NetFunction(Func<List<IValue>, Scope, IValue> body, string name)
        {
            Name = name;
            ArgumentNames = null;
            Body = body;
        }

        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public IValue Evaluate(List<IValue> arguments, Scope scope)
        {
            return Body(arguments, scope);
        }

        public virtual IValue Execute(ListNode list, Scope scope)
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }

    class MagicNetFunction : IFunction
    {
        private readonly Func<ListNode, Scope, IValue> Body;

        public MagicNetFunction(Func<ListNode, Scope, IValue> body, string name) 
        {
            Name = name;
            ArgumentNames = null;
            Body = body;
        }
        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public IValue Evaluate(List<IValue> arguments, Scope scope)
        {
            throw new NotImplementedException();
        }

        public virtual IValue Execute(ListNode list, Scope scope)
        {
            return Body(list, scope);
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }
}