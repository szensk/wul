using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class Meta
    {
        [MagicNetFunction("set-metamethod")]
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

        [MagicNetFunction("dump")]
        internal static IValue DumpValue(ListNode list, Scope scope)
        {
            IValue first = WulInterpreter.Interpret(list.Children[1], scope) ?? Value.Nil;
            SyntaxNode node = first.ToSyntaxNode(list.Parent);
            return new UString(node.ToString());
        }
    }
}
