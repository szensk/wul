using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    class NetFunction : IFunction
    {
        private readonly Func<List<IValue>, Scope, IValue> Body;

        public NetFunction(Func<List<IValue>, Scope, IValue> body, string name)
        {
            Name = name;
            ArgumentNames = null;
            Body = body;
            Metatype = FunctionMetaType.Instance;
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

        public MetaType Metatype { get; set; }
    }
}