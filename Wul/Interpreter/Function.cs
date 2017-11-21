using System;
using System.Collections.Generic;
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
            for (int i = 0; i < arguments.Count; ++i)
            {
                string argName = ArgumentNames[i];
                currentScope[argName] = arguments[i];
            }

            return WulInterpreter.Interpret(Body, currentScope);
        }

        public virtual void Execute(ListNode list, Scope scope)
        {
            throw new NotImplementedException();
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

        public virtual void Execute(ListNode list, Scope scope)
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

        public virtual void Execute(ListNode list, Scope scope)
        {
            Body(list, scope);
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }
}
