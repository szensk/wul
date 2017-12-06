using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class FunctionMetaType : MetaType
    {
        public static readonly FunctionMetaType Instance = new FunctionMetaType();

        private FunctionMetaType() : base(null)
        {
            //Invoke = new NetFunction(InvokeFunction, Instance.Invoke.Name);

            //AsString = new NetFunction(IdentityString, Instance.AsString.Name);
            //Type = new NetFunction(IdentityType, Instance.Type.Name);

            //Equal = new NetFunction(IdentityEqual, Instance.Equal.Name);
        }

        public static void SetMetaMethods()
        {
            Instance.Invoke = new NetFunction(InvokeFunction, Instance.Invoke.Name);

            Instance.AsString = new NetFunction(IdentityString, Instance.AsString.Name);
            Instance.Type = new NetFunction(IdentityType, Instance.Type.Name);

            Instance.Equal = new NetFunction(IdentityEqual, Instance.Equal.Name);

            Instance.InitializeDictionary();
        }

        public static IValue InvokeFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction) arguments[0];
            return function.Evaluate(arguments.Skip(1).ToList(), s);
        }
    }
}