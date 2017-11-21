using System;
using System.Linq;
using Wul.Parser;

namespace Wul.Interpreter
{
    public class WulInterpreter
    {
        public WulInterpreter()
        {
        }

        public static IValue Interpret(ProgramNode program)
        {
            IValue lastResult = null;
            foreach (var list in program.Expressions)
            {
                lastResult = Evaluate(list, Global.Scope);       
            }

            return lastResult;
        }

        internal static IValue Interpret(ListNode list, Scope currentScope)
        {
            return Evaluate(list, currentScope);
        }

        internal static IValue Interpret(SyntaxNode node, Scope currentScope)
        {
            switch (node)
            {
                case IdentifierNode i:
                    return Evaluate(i, currentScope);
                case NumericNode n:
                    return Evaluate(n);
                case BooleanNode b:
                    return Evaluate(b);
                case StringNode s:
                    return Evaluate(s);
                case ListNode l:
                    return Evaluate(l, currentScope);
                default:
                    throw new NotImplementedException();
            }
        }

        //TODO polymorphic dispatch for Evaluate

        private static IValue Evaluate(IdentifierNode identifier, Scope currentScope = null)
        {
            currentScope = currentScope ?? Global.Scope;

            return currentScope[identifier.Name];
        }

        private static IValue Evaluate(NumericNode numeric)
        {
            return (Number) numeric.Value;
        }

        private static IValue Evaluate(BooleanNode boolean)
        {
            return boolean.Value ? Bool.True : Bool.False;
        }

        private static IValue Evaluate(StringNode str)
        {
            return new UString(str.Value);
        }

        private static IValue Evaluate(ListNode list, Scope currentScope = null)
        {
            currentScope = currentScope ?? Global.Scope;

            if (list.Children.Count == 0) return new ListTable();

            var first = list.Children.First();
            IValue value = Value.Nil;
            if (first is IdentifierNode)
            {
                IdentifierNode identifier = first as IdentifierNode;
                string key = identifier.Name;
                value = currentScope[key];
            } 
            else if (first is ListNode)
            {
                value = Evaluate((ListNode) first, currentScope);
            }

            IFunction function = value as IFunction;
            //TODO make an IMagicFunction interface
            MagicFunction magicFunction = value as MagicFunction;
            MagicNetFunction magicNetFunction = value as MagicNetFunction;

            if (function != null && magicNetFunction == null && magicFunction == null)
            {
                //Invoke a regular function
                var remaining = list.Children.Skip(1).Select(node => Interpret(node, currentScope)).ToList();
                value = function.Evaluate(remaining, currentScope);
            }
            else if (magicFunction != null)
            {
                //Invoke a magic function
                value = magicFunction.Execute(list, currentScope);
            }
            else if (magicNetFunction != null)
            {
                //Invoke a magic function
                //Magic functions do not have their arguments evaluated, it's up the function to do so
                value = magicNetFunction.Execute(list, currentScope);
            }
            else
            {
                //Evaluate a list
                var remaining = list.Children.Select(node => Interpret(node, currentScope)).ToArray();
                if (remaining.Length > 1)
                {
                    value = new ListTable(remaining);
                }
                else if (remaining.Length == 1)
                {
                    value = remaining[0];
                }
            }
            
            return value;
        }
    }
}
