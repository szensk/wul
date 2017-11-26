using System.Collections.Generic;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class TypeMetaType : MetaType
    {
        public TypeMetaType()
        {
            Equal.Method = new NetFunction(IdentityEqual, Equal.Name);
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(NoType, Type.Name);
        }

        public IValue NoType(List<IValue> arguments, Scope s)
        {
            return Value.Nil;
        }

        public static readonly TypeMetaType Instance = new TypeMetaType();
    }
}
