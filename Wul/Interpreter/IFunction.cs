using System.Collections.Generic;

namespace Wul.Interpreter
{
    interface IFunction : IValue
    {
        string Name { get; }
        Scope Scope { get; }
        List<string> ArgumentNames { get; }
        IValue Evaluate(List<IValue> arguments);
    }
}