using System;
using System.Collections.Generic;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class IO
    {
        [NetFunction("print")]
        internal static IValue Print(List<IValue> list, Scope scope)
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
        }

        [NetFunction("clear")]
        internal static IValue Clear(List<IValue> list, Scope scope)
        {
            Console.Clear();
            return Value.Nil;
        }
    }
}
