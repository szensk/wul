using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    public class ValueException : Exception
    {
        public IValue Value { get; private set; }

        public ValueException(string message, IValue value) : base(message)
        {
            Value = value;
        }
    }

    public class Error
    {
        [NetFunction("error")]
        internal static IValue RaiseError(List<IValue> list, Scope scope)
        {
            IValue value = Value.Nil;
            if (list.Any())
            {
                value = list[0];
            }
            string message = value is WulString ? value.AsString() : null;
            throw new ValueException(message, value);
        }

        [MultiNetFunction("pcall")]
        internal static List<IValue> ProtectedCall(List<IValue> list, Scope scope)
        {
            try
            {
                var first = (IFunction) list[0];
                var arguments = list.Skip(1).ToList();
                var funcResult = first.Evaluate(arguments, scope);
                var results = Value.ListWith(Bool.True);
                results.AddRange(funcResult);
                return results;
            }
            catch (ValueException ve)
            {
                var results = Value.ListWith(Value.Nil);
                results.Add(ve.Value);
                return results;
            } 
            catch (Exception e)
            {
                var results = Value.ListWith(Value.Nil);
                results.Add(new WulString(e.Message));
                return results;
            }
        }
    }
}
