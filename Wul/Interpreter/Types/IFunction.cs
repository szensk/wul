using System.Collections.Generic;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    public interface IFunction : IValue
    {
        int Line { get; }
        string Name { get; }
        List<string> ArgumentNames { get; }
        List<IValue> Evaluate(List<IValue> arguments, Scope scope);
        List<IValue> Execute(ListNode list, Scope scope);
    }
}