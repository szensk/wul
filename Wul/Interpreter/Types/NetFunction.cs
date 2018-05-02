using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    sealed class NetFunction : IFunction
    {
        private readonly Func<List<IValue>, Scope, List<IValue>> Body;

        public NetFunction(Func<List<IValue>, Scope, List<IValue>> body, string name, int line = 0)
        {
            Line = line;
            Name = name;
            ArgumentNames = null;
            Body = body;
            MetaType = FunctionMetaType.Instance;
        }

        public static NetFunction FromSingle(Func<List<IValue>, Scope, IValue> body, string name, int line = 0)
        {
            return new NetFunction((list, scope) => Value.ListWith(body(list, scope)), name, line);
        }

        public int Line { get; }
        public string Name { get; }
        public string FileName => "Main";
        public List<string> ArgumentNames { get; }

        public List<IValue> Evaluate(List<IValue> arguments, Scope scope)
        {
            return Body(arguments, scope);
        }

        public List<IValue> Execute(ListNode list, Scope scope)
        {
            throw new NotImplementedException();
        }

        public WulType Type => FunctionType.Instance;

        public object ToObject()
        {
            //TODO
            return null;
        }

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }

        public MetaType MetaType { get; set; }
    }
}