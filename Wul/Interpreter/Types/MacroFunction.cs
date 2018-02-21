using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    sealed class MacroFunction : IFunction
    {
        private readonly ListNode Body;

        private Scope ParentScope { get; }

        public MacroFunction(ListNode body, string name, List<string> argumentNames, Scope parentScope)
        {
            Line = body.Line;
            Name = name;
            Body = body;
            ArgumentNames = argumentNames;
            ParentScope = parentScope.CloseScope(body);
            ParentScope.Parent = parentScope;
            MetaType = MagicFunctionMetaType.Instance;
        }

        public int Line { get; }
        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public List<IValue> Evaluate(List<IValue> arguments, Scope scope)
        {
            throw new NotImplementedException();
        }

        public List<IValue> Execute(ListNode list, Scope scope)
        {
            Scope currentScope = ParentScope.EmptyChildScope(macroScope: true);

            var arguments = list.Children.ToArray();

            //Bind arguments to names
            if (arguments.Any())
            {
                currentScope["self"] = arguments[0];
            }
            for (int i = 1; i <= ArgumentNames.Count; ++i)
            {
                string argName = ArgumentNames[i-1];
                if (argName == "...")
                {
                    currentScope[argName] = new ListNode(list, arguments.Skip(i).ToList());
                }
                else
                {
                    IValue argValue = i >= arguments.Length ? new IdentifierNode(list, "nil") : arguments[i];
                    currentScope[argName] = argValue;
                }
            }

            return WulInterpreter.Interpret(Body, currentScope);
        }

        public MetaType MetaType { get; set; }

        public WulType Type => MagicFunctionType.Instance;

        public object ToObject()
        {
            //TODO
            return null;
        }

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            List<SyntaxNode> listNodes = new List<SyntaxNode>
            {
                new IdentifierNode(parent, "defmacro"),
                new IdentifierNode(parent, Name)
            };
            ListNode args = new ListNode(parent, new List<SyntaxNode>());
            args.Children.AddRange(ArgumentNames.Select(a => new IdentifierNode(parent, a)));
            listNodes.Add(args);
            listNodes.Add(Body);
            return new ListNode(parent, listNodes);
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }
}