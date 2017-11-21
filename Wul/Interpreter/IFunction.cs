using System.Collections.Generic;
using Wul.Parser;

namespace Wul.Interpreter
{
    interface IFunction : IValue
    {
        string Name { get; }
        List<string> ArgumentNames { get; }
        IValue Evaluate(List<IValue> arguments, Scope scope);
        void Execute(ListNode list, Scope scope);
    }
}