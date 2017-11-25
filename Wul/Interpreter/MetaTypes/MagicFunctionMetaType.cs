using System.Collections.Generic;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.Interpreter.MetaTypes
{
    public class MagicFunctionMetaType : MetaType
    {
        public MagicFunctionMetaType()
        {
            InvokeMagic.Method = new NetFunction(InvokeMagicFunction, InvokeMagic.Name);

            AsString.Method = new NetFunction(IdentityString, AsString.Name);
        }

        public IValue InvokeMagicFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            ListNode listNode = (ListNode) arguments[1];
            return function.Execute(listNode, s);
        }
    }
}