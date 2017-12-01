using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    class MacroFunction : IFunction
    {
        public ListNode Body;

        public Scope ParentScope { get; }

        public MacroFunction(ListNode body, string name, List<string> argumentNames, Scope parentScope)
        {
            Name = name;
            Body = body;
            ArgumentNames = argumentNames;
            ParentScope = parentScope.CloseScope(body);
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
            Scope currentScope = ParentScope;

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

            IValue result = WulInterpreter.Interpret(Body, currentScope);
            return result;
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
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return $"Function[{Name}]";
        }
    }
}