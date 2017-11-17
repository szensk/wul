using System;
using System.Collections.Generic;
using Wul.Parser;

namespace Wul.Interpreter
{
    class Function : IFunction
    {
        public ListNode Body;

        public Function(ListNode body, string name, List<string> argumentNames, Scope scope)
        {
            Name = name;
            Body = body;
            Scope = scope;
            ArgumentNames = argumentNames;
        }

        public string Name { get; }
        public Scope Scope { get; }
        public List<string> ArgumentNames { get; }

        public IValue Evaluate(List<IValue> arguments)
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

        public NetFunction(Func<List<IValue>, Scope, IValue> body, string name, List<string> argumentNames)
        {
            Scope = Global.Scope;
            Name = name;
            ArgumentNames = argumentNames;
            Body = body;
        }

        public string Name { get; }
        public Scope Scope { get; }
        public List<string> ArgumentNames { get; }

        public IValue Evaluate(List<IValue> arguments)
        {
            return Body(arguments, Scope);
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }
}
