using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class TypeMetaType : MetaType
    {
        public static readonly TypeMetaType Instance = new TypeMetaType();

        private TypeMetaType()
        {
            Equal.Method = new NetFunction(AreEqual, Equal.Name);
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(NoType, Type.Name);
        }

        private IValue AreEqual(List<IValue> arguments, Scope s)
        {
            WulType first = (WulType) arguments.First();
            WulType second = (WulType) arguments.Skip(1).First();
            return first.RawType == second.RawType ? Bool.True : Bool.False;
        }

        private IValue NoType(List<IValue> arguments, Scope s)
        {
            return Value.Nil;
        }
    }
}
