using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

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

    class Function : IFunction
    {
        public ListNode Body;

        public Function(ListNode body, string name, List<string> argumentNames)
        {
            Name = name;
            Body = body;
            ArgumentNames = argumentNames;
            MetaType = FunctionMetaType.Instance;
        }

        public string Name { get; }
        public List<string> ArgumentNames { get; }

        public IValue Evaluate(List<IValue> arguments, Scope scope)
        {
            Scope currentScope = scope.EmptyChildScope();

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

            IValue result = WulInterpreter.Interpret(Body, currentScope);
            return result;
        }

        public virtual IValue Execute(ListNode list, Scope scope)
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
            //TODO
            return null;
        }

        public MetaType MetaType { get; set; }
    }

    class MagicFunction : IFunction
    {
        public ListNode Body;

        public MagicFunction(ListNode body, string name, List<string> argumentNames)
        {
            Name = name;
            Body = body;
            ArgumentNames = argumentNames;
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
            Scope currentScope = scope.EmptyChildScope();

            var arguments = list.Children.Skip(1).ToArray();

            //Bind arguments to names
            for (int i = 0; i < ArgumentNames.Count; ++i)
            {
                string argName = ArgumentNames[i];
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

    class NetFunction : IFunction
    {
        private readonly Func<List<IValue>, Scope, IValue> Body;

        public NetFunction(Func<List<IValue>, Scope, IValue> body, string name)
        {
            Name = name;
            ArgumentNames = null;
            Body = body;
            MetaType = FunctionMetaType.Instance;
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

        public MetaType MetaType { get; set; }
    }

    class MagicNetFunction : IFunction
    {
        private readonly Func<ListNode, Scope, IValue> Body;

        public MagicNetFunction(Func<ListNode, Scope, IValue> body, string name) 
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
