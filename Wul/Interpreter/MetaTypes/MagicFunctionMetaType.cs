using System.Collections.Generic;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.Interpreter.MetaTypes
{
    public class MagicFunctionMetaType : MetaType
    {
        public static readonly MagicFunctionMetaType Instance = new MagicFunctionMetaType();

        private MagicFunctionMetaType()
        {
            InvokeMagic.Method = new NetFunction(InvokeMagicFunction, ApplyMacro.Name);

            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);

            Equal.Method = new NetFunction(IdentityEqual, Equal.Name);
        }

        private IValue InvokeMagicFunction(List<IValue> arguments, Scope s)
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

            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);

            Equal.Method = new NetFunction(IdentityEqual, Equal.Name);
        }

        private IValue ApplyMacroFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction)arguments[0];
            ListNode listNode = (ListNode)arguments[1];
            return function.Execute(listNode, s);
        }
    }
}