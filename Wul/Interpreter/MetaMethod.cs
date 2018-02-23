using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.Interpreter
{
    public class MetaMethod 
    {
        public IFunction Method
        {
            get;
            set;
        }

        public bool IsDefined => Method != null;

        public string Name { get; }

        public MetaMethod(string name)
        {
            Name = name;
        }

        public MetaMethod(MetaMethod other)
        {
            Method = other.Method;
            Name = other.Name;
        }

        public List<IValue> Invoke(List<IValue> arguments, Scope s)
        {
            if (Method == null)
            {
                IValue lhs = arguments.First();
                throw new NotSupportedException($"Unable to invoke metamethod `{Name}` on type {lhs.Type.Name}");
            }

            if (Method.MetaType == FunctionMetaType.Instance)
            {
                return Method.Evaluate(arguments, s);
            }
            else
            {
                return Method.Execute((ListNode) arguments[1], s);
            }
        }
    }
}