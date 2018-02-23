using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.StdLib
{
    internal class Logical
    {
        [NetFunction("not")]
        internal static IValue Not(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Not.Invoke(list, scope).First();
        }

        [MagicFunction("or")]
        internal static IValue Or(ListNode list, Scope scope)
        {
            SyntaxNode[] nodes = list.Children.Skip(1).ToArray();

            IValue value = Value.Nil;
            foreach (SyntaxNode node in nodes)
            {
                value = node.EvalOnce(scope);
                if (!ReferenceEquals(value, Value.Nil) && !ReferenceEquals(value, Bool.False))
                {
                    return value;
                }
            }
            return value;
        }

        [MagicFunction("and")]
        internal static IValue And(ListNode list, Scope scope)
        {
            SyntaxNode[] nodes = list.Children.Skip(1).ToArray();

            IValue value = Value.Nil;
            foreach (SyntaxNode node in nodes)
            {
                value = node.EvalOnce(scope);
                if (ReferenceEquals(value, Value.Nil) || ReferenceEquals(value, Bool.False))
                {
                    return value;
                }
            }
            return value;
        }
    }
}
