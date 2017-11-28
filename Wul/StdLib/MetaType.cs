﻿using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class MetaType
    {
        [GlobalName("set-metamethod")]
        internal static IFunction SetMetaType = new MagicNetFunction((list, scope) =>
        {
            IValue first = list.Children[1].Eval(scope);
            IdentifierNode identifier = (IdentifierNode) list.Children[2];
            IValue function = list.Children[3].Eval(scope);

            string metaMethodName = identifier.Name;

            if (first is WulType type)
            {
                var metaMethod = type.DefaultMetaType.Get(metaMethodName);
                metaMethod.Method = function == Value.Nil ? null : (IFunction)function;
            }
            else
            {
                //Set the metamethod on the value
                var newMetaType = first.MetaType.Clone();

                var metaMethod = newMetaType.Get(metaMethodName);
                metaMethod.Method = function == Value.Nil ? null : (IFunction) function;

                first.MetaType = newMetaType;
            }

            return Value.Nil;
        }, "set-metamethod");
    }
}
