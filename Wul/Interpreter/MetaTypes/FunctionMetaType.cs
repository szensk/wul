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

            Instance.AsString.Method = new NetFunction(IdentityString, Instance.AsString.Name);
            Instance.Type.Method = new NetFunction(IdentityType, Instance.Type.Name);

            Instance.Equal.Method = new NetFunction(IdentityEqual, Instance.Equal.Name);
        }

        public static IValue InvokeFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            return function.Evaluate(arguments.Skip(1).ToList(), s);
        }
    }
}