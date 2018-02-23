using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    public class FunctionType : WulType
    {
        public FunctionType() : base("Function", typeof(IFunction))
        {
        }

        public static readonly FunctionType Instance = new FunctionType();
        public override MetaType DefaultMetaType => FunctionMetaType.Instance;
    }

    public class MagicFunctionType : WulType
    {
        public MagicFunctionType() : base("Function", typeof(IFunction))
        {
        }

        public static readonly MagicFunctionType Instance = new MagicFunctionType();
        public override MetaType DefaultMetaType => MagicFunctionMetaType.Instance;
    }

    sealed class Function : IFunction
    {
        private readonly ListNode Body;

        private Scope ParentScope { get; }

        public Function(ListNode body, string name, List<string> argumentNames, Scope parentScope)
        {
            Line = body.Line;
            Name = name;
            Body = body;
            ArgumentNames = argumentNames;
            ParentScope = parentScope.CloseScope(body);
            MetaType = FunctionMetaType.Instance;
        }

        public Function(Function f, Scope newScope)
        {
            Line = f.Line;
            Name = f.Name;
            Body = f.Body;
            ArgumentNames = f.ArgumentNames;
            ParentScope = newScope;
            MetaType = f.MetaType;
        }

        ~Function()
        {
            Debug.WriteLine($"Deleting function {Name}");
        }

        public int Line { get; }
        public string FileName => Body.File;
        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public List<IValue> Evaluate(List<IValue> arguments, Scope scope)
        {
            Scope currentScope = ParentScope.EmptyChildScope();

            currentScope["self"] = this;
            //Bind arguments to names
            for (int i = 0; i < ArgumentNames.Count; ++i)
            {
                string argName = ArgumentNames[i];
                if (argName == "...")
                {
                    currentScope[argName] = new ListTable(arguments.Skip(i));
                }
                else
                {
                    IValue argValue = i >= arguments.Count ? Value.Nil : arguments[i];
                    currentScope[argName] = argValue;
                } 
            }
            // If the function has no named parameters, then bind them to $0, $1...
            if (ArgumentNames.Count == 0)
            {
                currentScope["$args"] = new ListTable(arguments);
                for(int i = 0; i < arguments.Count; ++i)
                {
                    currentScope["$" + i] = arguments[i];
                }
            }

            return WulInterpreter.Interpret(Body, currentScope);
        }

        public List<IValue> Execute(ListNode list, Scope scope)
        {
            throw new NotImplementedException();
        }

        public WulType Type => FunctionType.Instance;

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            List<SyntaxNode> listNodes = new List<SyntaxNode>();
            listNodes.Add(new IdentifierNode(parent, "lambda"));
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

        public object ToObject()
        {
            IValue action() => Evaluate(Value.EmptyList, null).First();
            return (Func<IValue>) action;
        }

        public MetaType MetaType { get; set; }
    }
}
