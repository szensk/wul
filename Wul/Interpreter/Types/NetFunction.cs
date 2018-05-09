using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    internal class NetFunction : IFunction
    {
        private readonly Func<List<IValue>, Scope, IValue> Body;

        protected NetFunction(string name, int line, string fileName)
        {
            FileName = fileName ?? "Main";
            Line = line;
            Name = name;
            ArgumentNames = null;
            MetaType = FunctionMetaType.Instance;
        }

        public NetFunction(Func<List<IValue>, Scope, IValue> body, string name, int line = 0, string fileName = null)
            : this (name, line, fileName)
        {
            Body = body;
        }

        public int Line { get; }
        public string Name { get; }
        public string FileName { get; }
        public List<string> ArgumentNames { get; }

        public virtual List<IValue> Evaluate(List<IValue> arguments, Scope scope)
        {
            return Value.ListWith(Body(arguments, scope));
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

    internal class MultiNetFunction : NetFunction
    {
        private readonly Func<List<IValue>, Scope, List<IValue>> Body;

        public MultiNetFunction(Func<List<IValue>, Scope, List<IValue>> body, string name, int line = 0, string fileName = null)
            : base(name, line, fileName)
        {
            Body = body;
        }

        public override List<IValue> Evaluate(List<IValue> arguments, Scope scope)
        {
            return Body(arguments, scope);
        }
    }
}