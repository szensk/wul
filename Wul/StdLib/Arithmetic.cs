using System.Collections.Generic;
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
        }, "+", new List<string>());

        internal static IFunction LessThan = new NetFunction((list, scope) =>
        {
            var first = list.First() as Number;
            var second = list.Skip(1).First() as Number;
            if (first.Value < second.Value)
            {
                return (Number) 1;
            }
            else
            {
                return (Number) 0;
            }
        }, "<", new List<string>());

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
        }, "+", new List<string>());
    }
}
