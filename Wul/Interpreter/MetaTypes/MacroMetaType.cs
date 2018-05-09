using System.Collections.Generic;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.MetaTypes
{
    public class MacroMetaType : MetaType
    {
        public static readonly MacroMetaType Instance = new MacroMetaType();

        private MacroMetaType()
        {
            ApplyMacro.Method = new MultiNetFunction(ApplyMacroFunction, ApplyMacro.Name);

            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);

            Equal.Method = new NetFunction(IdentityEqual, Equal.Name);
        }

        private List<IValue> ApplyMacroFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction)arguments[0];
            ListNode listNode = (ListNode)arguments[1];
            return function.Execute(listNode, s);
        }
    }
}