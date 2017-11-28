using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Logical
    {
        [NetFunction("not")]
        internal static IValue Not(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Not.Invoke(list, scope);
        }

        [NetFunction("or")]
        internal static IValue Or(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Or.Invoke(list, scope);
        }

        [NetFunction("and")]
        internal static IValue And(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.And.Invoke(list, scope);
        }

        [NetFunction("xor")]
        internal static IValue Xor(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Xor.Invoke(list, scope);
        }
    }
}
