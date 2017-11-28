using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser;
using Wul.StdLib;

namespace Wul.Interpreter.MetaTypes
{
    public class NetObjectMetaType : MetaType
    {
        public static readonly NetObjectMetaType Instance = new NetObjectMetaType();

        private NetObjectMetaType()
        {
            //Equality
            Equal.Method = new NetFunction(IdentityEqual, Equal.Name);

            //List 
            At.Method = new NetFunction(AtKey, At.Name);
            Set.Method = new NetFunction(SetKey, Set.Name);

            //Other
            AsString.Method = new NetFunction(ConvertToString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
            InvokeMagic.Method = new NetFunction(InvokeMethod, Invoke.Name);
        }

        public IValue AtKey(List<IValue> arguments, Scope s)
        {
            ListNode list = (ListNode) arguments[0];
            NetObject netObj = (NetObject) list.Children[1].Eval(s);
            IdentifierNode index = (IdentifierNode) list.Children[2];

            return netObj.Get(index.Name);
        }

        public IValue SetKey(List<IValue> arguments, Scope s)
        {
            ListNode list = (ListNode)arguments[0];
            NetObject netObj = (NetObject)list.Children[1].Eval(s);
            IdentifierNode index = (IdentifierNode)list.Children[2];
            IValue value = list.Children[3].Eval(s);

            netObj.Set(index.Name, value);

            return value;
        }

        public IValue ConvertToString(List<IValue> arguments, Scope s)
        {
            NetObject obj = (NetObject) arguments[0];

            return new UString(obj.AsString());
        }

        public IValue InvokeMethod(List<IValue> arguments, Scope s)
        {
            NetObject netObj = (NetObject)arguments[0];
            ListNode listNode = (ListNode)arguments[1];
            IdentifierNode name = (IdentifierNode)listNode.Children[1];
            var parameters = listNode.Children.Skip(2).Select(c => c.Eval(s)).ToArray();
            if (parameters.Any())
            {
                return netObj.Call(name.Name, parameters);
            }
            return netObj.Call(name.Name);
        }
    }
}
