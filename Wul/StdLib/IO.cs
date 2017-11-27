using System;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class IO
    {
        [GlobalName("print")]
        internal static IFunction Print = new NetFunction((list, scope) =>
        {
            foreach (var value in list)
            {
                if (value is UString)
                {
                    Console.WriteLine($"'{value.AsString()}'");
                }
                else
                {
                    Console.WriteLine(value.AsString());
                }
            }
            return Value.Nil;
        }, "print");

        [GlobalName("clear")]
        internal static IFunction Clear = new NetFunction((list, scope) =>
        {
            Console.Clear();
            return Value.Nil;
        }, "clear");
    }
}
