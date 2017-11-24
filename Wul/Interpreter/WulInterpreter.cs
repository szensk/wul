using System;
using System.Linq;
using Wul.Parser;
using Wul.StdLib;

namespace Wul.Interpreter
{
    public class WulInterpreter
    {
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

        //TODO polymorphic dispatch for Evaluate
        internal static IValue Interpret(SyntaxNode node, Scope currentScope)
        {
            switch (node)
            {
                case CommentNode c:
                    return Evaluate(c);
                case IdentifierNode i:
                    return Evaluate(i, currentScope);
                case NumericNode n:
                    return Evaluate(n);
                case BooleanNode b:
                    return Evaluate(b);
                case StringNode s:
                    return Evaluate(s, currentScope);
                case ListNode l:
                    return Evaluate(l, currentScope);
                default:
                    throw new NotImplementedException();
            }
        }


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

        private static IValue Evaluate(CommentNode comment)
        {
            return null;
        }

        private static IValue Evaluate(StringNode str, Scope currentScope = null)
        {
            return new UString(str.Value(currentScope));
        }

        private static IValue Evaluate(ListNode list, Scope currentScope = null)
        {
            currentScope = currentScope ?? Global.Scope;

            if (list.Children.Count == 0) return new ListTable();

            var first = list.Children.First();
            IValue value = Value.Nil;

            //TODO Really this is an recursive call to Interpret
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

            bool isFunction = value.MetaType?.Invoke?.IsDefined ?? false;
            bool isMagicFunction = value.MetaType?.InvokeMagic?.IsDefined ?? false;
            if (isFunction)
            {
                IFunction function = (IFunction) value;
                var remaining = list.Children.Skip(1);

                var evaluatedRemaining = remaining
                    .Select(node => Interpret(node, currentScope))
                    .Where(v => v != null)
                    .ToList();

                value = function.Evaluate(evaluatedRemaining, currentScope);
            }
            else if (isMagicFunction)
            {
                //Magic functions do not have their arguments 
                IFunction function = (IFunction) value;
                value = function.Execute(list, currentScope);
            }
            else
            {
                //Evaluate a list
                var remaining = list.Children
                    .Where(node => !(node is CommentNode))
                    .Select(node => Interpret(node, currentScope))
                    .ToArray();

                if (remaining.Length > 0)
                {
                    value = new ListTable(remaining);
                }
            }
            
            return value;
        }
    }
}
