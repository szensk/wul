using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class Comparison
    {
        internal static IFunction Equal = new NetFunction((list, scope) =>
        {
            IValue first = list[0];
            IValue second = list[1];
            //TODO make all IValue override Equals
            bool equal = first.Equals(second);
            return equal ? Bool.True : Bool.False;
        }, "=");

        internal static IFunction LessThan = new NetFunction((list, scope) =>
        {
            Number first = list[0] as Number;
            Number second = list[1] as Number;
            bool lessThan = first.Value < second.Value;
            return lessThan ? Bool.True : Bool.False;
        }, "<");
    }
}
