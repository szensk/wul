using System.Linq;
using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class Arithmetic
    {
        internal static IFunction Add = new NetFunction((list, scope) =>
        {
            var numbers = list.Select(x => x as Number).Where(x => x != null);
            if (!numbers.Any())
            {
                return Value.Nil;
            }
            double sum = numbers.Sum(x => x.Value);
            return (Number) sum;
        }, "+");

        internal static IFunction Subtract = new NetFunction((list, scope) =>
        {
            var first = list.First() as Number;
            var numbers = list.Skip(1).Select(x => x as Number).Where(x => x != null);
            if (!numbers.Any())
            {
                return first;
            }
            double sum = numbers.Sum(x => x.Value);
            return (Number) (first.Value - sum);
        }, "-");

        internal static IFunction Multiply = new NetFunction((list, scope) =>
        {
            var numbers = list.Select(x => x as Number).Where(x => x != null).ToArray();
            if (!numbers.Any())
            {
                return Value.Nil;
            }
            double multiplied = numbers[0];
            for (int i = 1; i < numbers.Length; ++i)
            {
                multiplied *= numbers[i];
            }
            return (Number)multiplied;
        }, "*");
    }
}
