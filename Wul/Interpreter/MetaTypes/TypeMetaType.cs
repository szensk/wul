using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class TypeMetaType : MetaType
    {
        public static readonly TypeMetaType Instance = new TypeMetaType();

        private TypeMetaType() : base(null)
        {
            Equal = new NetFunction(AreEqual, Equal.Name);
            AsString = new NetFunction(IdentityString, AsString.Name);
            Type = new NetFunction(NoType, Type.Name);

            InitializeDictionary();
        }

        protected IValue AreEqual(List<IValue> arguments, Scope s)
        {
            WulType first = (WulType) arguments.First();
            WulType second = (WulType) arguments.Skip(1).First();
            return first.RawType == second.RawType ? Bool.True : Bool.False;
        }

        public IValue NoType(List<IValue> arguments, Scope s)
        {
            return Value.Nil;
        }
    }
}
