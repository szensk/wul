using System;
using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class IO
    {
        internal static IFunction Print = new NetFunction((list, scope) =>
        {
            foreach (var value in list)
            {
                Console.WriteLine(value.AsString());
            }
            return Value.Nil;
        }, "print");

        internal static IFunction Clear = new NetFunction((list, scope) =>
        {
            Console.Clear();
            return Value.Nil;
        }, "clear");
    }
}
