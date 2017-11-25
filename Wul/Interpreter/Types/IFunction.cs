using System.Collections.Generic;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    public interface IFunction : IValue
    {
        string Name { get; }
        List<string> ArgumentNames { get; }
        IValue Evaluate(List<IValue> arguments, Scope scope);
        IValue Execute(ListNode list, Scope scope);
    }
}