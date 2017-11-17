using System;
using System.Collections.Generic;
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
        }, "print", new List<string>());

        internal static IFunction Clear = new NetFunction((list, scope) =>
        {
            Console.Clear();
            return Value.Nil;
        }, "clear", new List<string>());
    }
}
