using System.Collections.Generic;
using System.Linq;

namespace Wul.Interpreter
{
    public class FunctionMetaType : MetaType
    {
        public FunctionMetaType()
        {
            Invoke.Method = new NetFunction(InvokeFunction, Invoke.Name);

            AsString.Method = new NetFunction(IdentityString, AsString.Name);
        }

        public IValue InvokeFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            return function.Evaluate(arguments.Skip(1).ToList(), s);
        }
    }
}