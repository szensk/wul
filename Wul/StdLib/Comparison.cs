﻿using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Comparison
    {
        [NetFunction("=")]
        internal static IValue Equal(List<IValue> list, Scope scope)
        {
            IValue first = list[0];

            if (first.Metatype != null)
            {
                return first.Metatype.Equal.Invoke(list, scope);
            }
            else
            {
                return ReferenceEquals(first, list[1]) ? Bool.True : Bool.False;
            }
        }

        [NetFunction("<")]
        internal static IValue LessThan(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number) first.Metatype.Compare.Invoke(list, scope);
            int value = (int) comparison.Value;
            return value == -1 ? Bool.True : Bool.False;
        }

        [NetFunction("<=")]
        internal static IValue LessThanEqualTo(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number)first.Metatype.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
        
            return value == -1 || value == 0 ? Bool.True : Bool.False;
        }

        [NetFunction(">")]
        internal static IValue GreaterThan(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number)first.Metatype.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;
            return value == 1 ? Bool.True : Bool.False;
        }

        [NetFunction(">=")]
        internal static IValue GreaterThanEqualTo(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number)first.Metatype.Compare.Invoke(list, scope);
            int value = (int)comparison.Value;

            return value == 1 || value == 0 ? Bool.True : Bool.False;
        }

        [NetFunction("compare")]
        internal static IValue Compare(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.Metatype.Compare.Invoke(list, scope);
        }
    }
}
