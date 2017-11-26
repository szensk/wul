using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Comparison
    {
        internal static IFunction Equal = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Equal.Invoke(list, scope);
        }, "=");

        internal static IFunction LessThan = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number) first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
            return value == -1 ? Bool.True : Bool.False;
        }, "<");

        internal static IFunction LessThanEqualTo = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
        
            return value == -1 || value == 0 ? Bool.True : Bool.False;
        }, "<=");

        internal static IFunction GreaterThan = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
            return value == 1 ? Bool.True : Bool.False;
        }, ">");

        internal static IFunction GreaterThanEqualTo = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;

            return value == 1 || value == 0 ? Bool.True : Bool.False;
        }, ">=");

        internal static IFunction Compare = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Compare.Invoke(list, scope);
        }, "compare");
    }
}
