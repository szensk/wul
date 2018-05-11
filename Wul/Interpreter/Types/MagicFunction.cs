using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    internal class MagicFunction : IFunction
    {
        private readonly Func<ListNode, Scope, IValue> Body;

        protected MagicFunction(string name, int line, string fileName)
        {
            FileName = fileName ?? "Main";
            Line = line;
            Name = name;
            ArgumentNames = null;
            MetaType = MagicFunctionMetaType.Instance;
        }

        public MagicFunction(Func<ListNode, Scope, IValue> body, string name, int line = 0, string fileName = null)
            : this (name, line, fileName)
        {
            Body = body;
        }

        public int Line { get; }
        public string Name { get; }
        public string FileName { get; }
        public List<string> ArgumentNames { get; }

        public List<IValue> Evaluate(List<IValue> arguments, Scope scope)
        {
            throw new NotImplementedException();
        }

        public virtual List<IValue> Execute(ListNode list, Scope scope)
        {
            return Value.ListWith(Body(list, scope));
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

    internal class MultiMagicFunction : MagicFunction
    {
        private readonly Func<ListNode, Scope, List<IValue>> Body;

        public MultiMagicFunction(Func<ListNode, Scope, List<IValue>> body, string name, int line = 0, string fileName = null)
            : base(name, line, fileName)
        {
            Body = body;
        }

        public override List<IValue> Execute(ListNode list, Scope scope)
        {
            return Body(list, scope);
        }
    }
}