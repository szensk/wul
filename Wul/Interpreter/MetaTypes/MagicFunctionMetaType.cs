using System.Collections.Generic;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.MetaTypes
{
    public class MagicFunctionMetaType : MetaType
    {
        public static readonly MagicFunctionMetaType Instance = new MagicFunctionMetaType();

        private MagicFunctionMetaType()
        {
            InvokeMagic.Method = new NetFunction(InvokeMagicFunction, ApplyMacro.Name);

            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(IdentityType, Type.Name);

            Equal.Method = NetFunction.FromSingle(IdentityEqual, Equal.Name);
        }

        private List<IValue> InvokeMagicFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            ListNode listNode = (ListNode) arguments[1];
            return function.Execute(listNode, s);
        }
    }

    public class MacroMetaType : MetaType
    {
        public static readonly MacroMetaType Instance = new MacroMetaType();

        private MacroMetaType()
        {
            ApplyMacro.Method = new NetFunction(ApplyMacroFunction, ApplyMacro.Name);

            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(IdentityType, Type.Name);

            Equal.Method = NetFunction.FromSingle(IdentityEqual, Equal.Name);
        }

        private List<IValue> ApplyMacroFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction)arguments[0];
            ListNode listNode = (ListNode)arguments[1];
            return function.Execute(listNode, s);
        }
    }
}