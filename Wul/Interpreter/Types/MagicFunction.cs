using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    class MagicFunction : IFunction
    {
        private readonly Func<ListNode, Scope, IValue> Body;

        public MagicFunction(Func<ListNode, Scope, IValue> body, string name) 
        {
            Name = name;
            ArgumentNames = null;
            Body = body;
            MetaType = MagicFunctionMetaType.Instance;
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