using System.Collections.Generic;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    public interface IFunction : IValue
    {
        string Name { get; }
        List<string> ArgumentNames { get; }
        List<IValue> Evaluate(List<IValue> arguments, Scope scope);
        List<IValue> Execute(ListNode list, Scope scope);
    }
}