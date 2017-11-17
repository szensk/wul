using System;
using System.Linq;
using Wul.Parser;

namespace Wul.Interpreter
{
    public class WulInterpreter
    {
        private readonly Scope Globals;

        public WulInterpreter()
        {
            Globals = Global.Scope;
            Global.RegisterDefaultFunctions();
        }

        public IValue Interpret(ProgramNode program)
        {
            IValue lastResult = null;
            foreach (var list in program.Expressions)
            {
                lastResult = Evaluate(list);       
            }

            return lastResult;
        }

        //TODO polymorphic dispatch 
        private IValue Evaluate(IdentifierNode identifier)
        {
            if (identifier.Name.StartsWith("`"))
            {
                return new UString(identifier.Name.Replace("`", ""));
            }
            return Globals[identifier.Name];
        }

        private IValue Evaluate(NumericNode numeric)
        {
            return (Number) numeric.Value;
        }

        private IValue Evaluate(ListNode list)
        {
            var first = list.Children.First();
            IValue value = Value.Nil;
            if (first is IdentifierNode)
            {
                IdentifierNode identifier = first as IdentifierNode;
                string key = identifier.Name;
                value = Globals[key];
            } 
            else if (first is ListNode)
            {
                value = Evaluate((ListNode) first);
            }

            IFunction function = value as IFunction;
            if (function != null)
            {
                var remaining = list.Children.Skip(1).Select(node =>
                {
                    switch (node)
                    {
                        case IdentifierNode i:
                            return Evaluate(i);
                        case NumericNode n:
                            return Evaluate(n);
                        case ListNode l:
                            return Evaluate(l);
                        default:
                            throw new NotImplementedException();
                    }
                }).ToList();
                value = function.Evaluate(remaining);
            }
            else
            {
                var remaining = list.Children.Select(node =>
                {
                    switch (node)
                    {
                        case IdentifierNode i:
                            return Evaluate(i);
                        case NumericNode n:
                            return Evaluate(n);
                        case ListNode l:
                            return Evaluate(l);
                        default:
                            throw new NotImplementedException();
                    }
                }).ToArray();
                value = new ListTable(remaining);
            }
            
            return value;
        }
    }
}
