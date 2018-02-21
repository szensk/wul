using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    sealed class MagicFunction : IFunction
    {
        private readonly Func<ListNode, Scope, List<IValue>> Body;

        public MagicFunction(Func<ListNode, Scope, List<IValue>> body, string name) 
        {
            Name = name;
            ArgumentNames = null;
            Body = body;
            MetaType = MagicFunctionMetaType.Instance;
        }

        public static MagicFunction FromSingle(Func<ListNode, Scope, IValue> body, string name)
        {
            return new MagicFunction((list, scope) => Value.ListWith(body(list, scope)), name);
        }

        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public List<IValue> Evaluate(List<IValue> arguments, Scope scope)
        {
            throw new NotImplementedException();
        }

        public List<IValue> Execute(ListNode list, Scope scope)
        {
            return Body(list, scope);
        }

        public WulType Type => MagicFunctionType.Instance;

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