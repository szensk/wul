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
    }
}
