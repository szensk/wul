using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser;
using Wul.StdLib;

namespace Wul.Interpreter
{
    public class WulInterpreter
    {
        public static IValue Interpret(ProgramNode program, Scope scope = null)
        {
            Scope currentScope = scope ?? Global.Scope.EmptyChildScope();
            IValue lastResult = null;
            foreach (var list in program.Expressions)
            {
                lastResult = Evaluate(list, currentScope);       
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
                case RangeNode r:
                    return Evaluate(r, currentScope);
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

        private static IValue Evaluate(RangeNode rangeNode, Scope currentScope = null)
        {
            currentScope = currentScope ?? Global.Scope;
            var arguments = rangeNode.Children.Select(c => c.Eval(currentScope));
            return StdLib.Range.RangeFromArguments(arguments.ToList(), currentScope);
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
            else if (first is RangeNode)
            {
                value = Evaluate((RangeNode) first, currentScope);
            }

            bool isFunction = value.MetaType?.Invoke?.IsDefined ?? false;
            bool isMagicFunction = value.MetaType?.InvokeMagic?.IsDefined ?? false;
            bool isMacroFunction = value.MetaType?.ApplyMacro?.IsDefined ?? false;
            if (isFunction)
            {
                var evalutedList = list.Children
                    .Skip(1)
                    .Select(node => Interpret(node, currentScope))
                    .Where(v => v != null)
                    .ToList();

                evalutedList = UnpackList.Replace(evalutedList);

                var function = value.MetaType.Invoke.Method;

                var finalList = new List<IValue> {value};
                finalList.AddRange(evalutedList);

                value = function.Evaluate(finalList, currentScope);
            }
            else if (isMagicFunction)
            {
                //Magic functions are passed syntax nodes, not fully evaluated arguments
                var function = value.MetaType.InvokeMagic;
                value = function.Invoke(new List<IValue>{value, list}, currentScope);
            }
            else if (isMacroFunction)
            {
                var function = value.MetaType.ApplyMacro;
                //TODO evaluate macro arguments first
                value = function.Invoke(new List<IValue>{value, list}, currentScope);
                //TODO how to avoid the ToSyntaxNode step?
                //Problem is that ListNodes are evaluated to ListTable
                SyntaxNode node = value as SyntaxNode ?? value.ToSyntaxNode(list.Parent);
                System.Diagnostics.Debug.WriteLine($"macro -> {node}");
                value = Interpret(node, currentScope) ?? Value.Nil;
            }
            else
            {
                //Evaluate a list
                var remaining = list.Children
                    .Where(node => !(node is CommentNode))
                    .Select(node => Interpret(node, currentScope))
                    .ToList();

                remaining = UnpackList.Replace(remaining);

                if (remaining.Count > 0)
                {
                    value = new ListTable(remaining);
                }
            }
            
            return value;
        }
    }
}
