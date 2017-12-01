using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class Meta
    {
        [MagicFunction("set-metamethod")]
        internal static IValue SetMetaType(ListNode list, Scope scope)
        {
            IValue first = list.Children[1].Eval(scope);
            IdentifierNode identifier = (IdentifierNode) list.Children[2];
            IValue function = list.Children[3].Eval(scope);

            string metaMethodName = identifier.Name;

            if (first is WulType type)
            {
                var metaMethod = type.DefaultMetaType.Get(metaMethodName);
                metaMethod.Method = ReferenceEquals(function, Value.Nil) ? null : (IFunction)function;
            }
            else
            {
                //Set the metamethod on the value
                var newMetaType = first.MetaType.Clone();

                var metaMethod = newMetaType.Get(metaMethodName);
                metaMethod.Method = ReferenceEquals(function, Value.Nil) ? null : (IFunction) function;

                first.MetaType = newMetaType;
            }

            return Value.Nil;
        }

        [MagicFunction("dump")]
        internal static IValue DumpValue(ListNode list, Scope scope)
        {
            IValue first = WulInterpreter.Interpret(list.Children[1], scope) ?? Value.Nil;
            SyntaxNode node = first.ToSyntaxNode(list.Parent);
            return new UString(node.ToString());
        }

        [MagicFunction("eval")]
        internal static IValue Evaluate(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0].Eval(scope);
        }

        [MagicFunction("quote")]
        internal static IValue Quote(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0];
        }

        [MagicFunction("unquote")]
        internal static IValue Unquote(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0].EvalOnce(scope);
        }

        [MagicFunction("defmacro")]
        internal static IValue DefineMagicFunction(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode)children[0];
            string name = nameIdentifier.Name;

            var arguments = (ListNode)children[1];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode)children[2];
            scope[name] = Value.Nil;
            var function = new MacroFunction(body, name, argNames.ToList(), scope);
            function.MetaType = MacroMetaType.Instance;
            scope[name] = function;

            return function;
        }
    }
}
