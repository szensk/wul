using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    // ReSharper disable once InconsistentNaming
    internal class IO
    {
        [NetFunction("print")]
        internal static IValue Print(List<IValue> list, Scope scope)
        {
            foreach (IValue value in list)
            {
                string stringValue;
                if (value.MetaType?.AsString?.IsDefined ?? false)
                {
                    WulString ustring = (WulString) value.MetaType.AsString.Invoke(new List<IValue> {value}, scope).First();
                    stringValue = ustring.Value;
                }
                else
                {
                    stringValue = value.AsString();
                }

                Console.WriteLine(stringValue);
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
