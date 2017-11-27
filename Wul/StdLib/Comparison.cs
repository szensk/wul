using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Comparison
    {
        [GlobalName("=")]
        internal static IFunction Equal = new NetFunction((list, scope) =>
        {
            IValue first = list[0];

            if (first.MetaType != null)
            {
                return first.MetaType.Equal.Invoke(list, scope);
            }
            else
            {
                return first == list[1] ? Bool.True : Bool.False;
            }
        }, "=");

        [GlobalName("<")]
        internal static IFunction LessThan = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number) first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
            return value == -1 ? Bool.True : Bool.False;
        }, "<");

        [GlobalName("<=")]
        internal static IFunction LessThanEqualTo = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
        
            return value == -1 || value == 0 ? Bool.True : Bool.False;
        }, "<=");

        [GlobalName(">")]
        internal static IFunction GreaterThan = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
            return value == 1 ? Bool.True : Bool.False;
        }, ">");

        [GlobalName(">=")]
        internal static IFunction GreaterThanEqualTo = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;

            return value == 1 || value == 0 ? Bool.True : Bool.False;
        }, ">=");

        [GlobalName("compare")]
        internal static IFunction Compare = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Compare.Invoke(list, scope);
        }, "compare");
    }
}
