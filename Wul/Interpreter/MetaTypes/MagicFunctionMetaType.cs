using System.Collections.Generic;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.Interpreter.MetaTypes
{
    public class MagicFunctionMetaType : MetaType
    {
        public static readonly MagicFunctionMetaType Instance = new MagicFunctionMetaType();

        private MagicFunctionMetaType() : base(null)
        {
            InvokeMagic = new NetFunction(InvokeMagicFunction, InvokeMagic.Name);

            AsString = new NetFunction(IdentityString, AsString.Name);
            Type = new NetFunction(IdentityType, Type.Name);

            InitializeDictionary();
        }

        public IValue InvokeMagicFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            ListNode listNode = (ListNode) arguments[1];
            return function.Execute(listNode, s);
        }
    }

    public class MacroMetaType : MetaType
    {
        public static readonly MacroMetaType Instance = new MacroMetaType();

        private MacroMetaType() : base(null)
        {
            ApplyMacro = new NetFunction(ApplyMacroFunction, ApplyMacro.Name);

            AsString = new NetFunction(IdentityString, AsString.Name);
            Type = new NetFunction(IdentityType, Type.Name);

            InitializeDictionary();
        }

        public IValue ApplyMacroFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            ListNode listNode = (ListNode) arguments[1];
            return function.Execute(listNode, s);
        }
    }
}