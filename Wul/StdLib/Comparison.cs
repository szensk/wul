﻿using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.StdLib.Attribute;

namespace Wul.StdLib
{
    [StdLib]
    internal class Comparison
    {
        [NetFunction("=")]
        internal static IValue Equal(List<IValue> list, Scope scope)
        {
            IValue first = list[0];

            if (first.MetaType != null)
            {
                return first.MetaType.Equal.Invoke(list, scope).First();
            }
            else
            {
                return ReferenceEquals(first, list[1]) ? Bool.True : Bool.False;
            }
        }

        [NetFunction("is")]
        internal static IValue IsInstance(List<IValue> list, Scope scope)
        {
            IValue first = list[0];
            foreach (IValue val in list.Skip(1))
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (val != first) return Bool.False;
            }
            return Bool.True;
        }

        [NetFunction("<")]
        internal static IValue LessThan(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number) first.MetaType.Compare.Invoke(list, scope).First();
            int value = (int) comparison.Value;
            return value == -1 ? Bool.True : Bool.False;
        }

        [NetFunction("<=")]
        internal static IValue LessThanEqualTo(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope).First();
            int value = (int)comparison.Value;
        
            return value == -1 || value == 0 ? Bool.True : Bool.False;
        }

        [NetFunction(">")]
        internal static IValue GreaterThan(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope).First();
            int value = (int)comparison.Value;
            return value == 1 ? Bool.True : Bool.False;
        }

        [NetFunction(">=")]
        internal static IValue GreaterThanEqualTo(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number comparison = (Number)first.MetaType.Compare.Invoke(list, scope).First();
            int value = (int)comparison.Value;

            return value == 1 || value == 0 ? Bool.True : Bool.False;
        }

        [NetFunction("compare")]
        internal static IValue Compare(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Compare.Invoke(list, scope).First();
        }
    }
}
