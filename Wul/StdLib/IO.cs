using System;
using System.Collections.Generic;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class IO
    {
        [GlobalName("print")]
        internal static IFunction Print = new NetFunction((list, scope) =>
        {
            foreach (IValue value in list)
            {
                string stringValue = "";
                if (value.MetaType?.AsString?.IsDefined ?? false)
                {
                    UString ustring = (UString) value.MetaType.AsString.Invoke(new List<IValue> {value}, scope);
                    stringValue = ustring.Value;
                }
                if (value is UString)
                {
                    Console.WriteLine($"'{stringValue}'");
                }
                else
                {
                    Console.WriteLine(stringValue);
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
