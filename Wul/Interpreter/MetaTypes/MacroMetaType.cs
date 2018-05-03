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
            ApplyMacro.Method = new NetFunction(ApplyMacroFunction, ApplyMacro.Name);

            AsString.Method = NetFunction.FromSingle(MetaType.IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(MetaType.IdentityType, Type.Name);

            Equal.Method = NetFunction.FromSingle(MetaType.IdentityEqual, Equal.Name);
        }

        private List<IValue> ApplyMacroFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction)arguments[0];
            ListNode listNode = (ListNode)arguments[1];
            return function.Execute(listNode, s);
        }
    }
}