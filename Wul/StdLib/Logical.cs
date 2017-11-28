using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    internal class Logical
    {
        [NetFunction("not")]
        internal static IValue Not(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Not.Invoke(list, scope);
        }

        [MagicNetFunction("or")]
        internal static IValue Or(ListNode list, Scope scope)
        {
            SyntaxNode[] nodes = list.Children.Skip(1).ToArray();

            IValue value = Value.Nil;
            foreach (SyntaxNode node in nodes)
            {
                value = node.Eval(scope);
                if (value != Value.Nil && value != Bool.False)
                {
                    return value;
                }
            }
            return value;
        }

        [MagicNetFunction("and")]
        internal static IValue And(ListNode list, Scope scope)
        {
            SyntaxNode[] nodes = list.Children.Skip(1).ToArray();

            IValue value = Value.Nil;
            foreach (SyntaxNode node in nodes)
            {
                value = node.Eval(scope);
                if (value == Value.Nil || value == Bool.False)
                {
                    return value;
                }
            }
            return value;
        }

        [NetFunction("xor")]
        internal static IValue Xor(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Xor.Invoke(list, scope);
        }
    }
}
