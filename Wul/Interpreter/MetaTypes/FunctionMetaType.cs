using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class FunctionMetaType : MetaType
    {
        public static readonly FunctionMetaType Instance = new FunctionMetaType();

        private FunctionMetaType()
        {
        }

        public static void SetMetaMethods()
        {
            Instance.Invoke.Method = new NetFunction(InvokeFunction, Instance.Invoke.Name);

            Instance.AsString.Method = NetFunction.FromSingle(IdentityString, Instance.AsString.Name);
            Instance.Type.Method = NetFunction.FromSingle(IdentityType, Instance.Type.Name);

            Instance.Equal.Method = NetFunction.FromSingle(IdentityEqual, Instance.Equal.Name);
        }

        private static List<IValue> InvokeFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            return function.Evaluate(arguments.Skip(1).ToList(), s);
        }
    }
}