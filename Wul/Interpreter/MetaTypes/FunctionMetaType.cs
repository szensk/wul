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
            Invoke.Method = new NetFunction(InvokeFunction, Invoke.Name);

            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
        }

        public IValue InvokeFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            return function.Evaluate(arguments.Skip(1).ToList(), s);
        }
    }
}