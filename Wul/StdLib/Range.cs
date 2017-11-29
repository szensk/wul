using System.Collections.Generic;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    class Range
    {
        [NetFunction("range")]
        internal static IValue RangeFromArguments(List<IValue> list, Scope scope)
        {
            Number start = (Number) list[0];

            double? end = null;
            double? increment = null;

            if (list.Count == 1)
            {
                end = start.Value;
            }
            else if (!ReferenceEquals(list[1], Value.Nil))
            {
                end = ((Number) list[1]).Value;
            }

            if (list.Count < 3)
            {
                if (start.Value < end)
                {
                    increment = 1;
                }
                else
                {
                    increment = -1;
                }
            }
            else if (list.Count == 3 && !ReferenceEquals(list[2], Value.Nil))
            {
                increment = ((Number) list[2]).Value;
            }

            return new Interpreter.Types.Range(start.Value, end, increment);
        }
    }
}
