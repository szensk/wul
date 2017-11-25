using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class MetaType
    {
        internal static IFunction SetMetaType = new MagicNetFunction((list, scope) =>
        {
            IValue first = list.Children[1].Eval(scope);
            IdentifierNode identifier = (IdentifierNode) list.Children[2];
            IFunction function = (IFunction) list.Children[3].Eval(scope);

            string metaMethodName = identifier.Name;

            var newMetaType = first.MetaType.Clone();

            var metaMethod = newMetaType.Get(metaMethodName);
            metaMethod.Method = function;

            return Value.Nil;
        }, "set-metamethod");
    }
}
