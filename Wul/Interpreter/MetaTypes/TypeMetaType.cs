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
            Equal.Method = NetFunction.FromSingle(AreEqual, Equal.Name);
            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(NoType, Type.Name);
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
