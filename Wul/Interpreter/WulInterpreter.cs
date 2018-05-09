using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.StdLib;

namespace Wul.Interpreter
{
    public class StackInformation
    {
        public string FileName;
        public int Line;
        public string FunctionName;
        public string CallingFile;
        public int CallingLine;

        public override string ToString()
        {
            return $"{FileName} line {Line} {FunctionName} called {CallingFile ?? "Main"} line {CallingLine}";
        }
    }

    public class WulInterpreter
    {
        public static readonly Stack<StackInformation> CallStack = new Stack<StackInformation>();

        public static List<IValue> Interpret(ProgramNode program, Scope scope = null)
        {
            Scope currentScope = scope ?? Global.Scope.EmptyChildScope();
            List<IValue> lastResult = null;
            foreach (var list in program.Expressions)
            {
                lastResult = Evaluate(list, currentScope);       
            }

            return lastResult;
        }

        internal static List<IValue> Interpret(ListNode list, Scope currentScope)
        {
            return Evaluate(list, currentScope);
        }

        //TODO polymorphic dispatch for Evaluate
        internal static List<IValue> Interpret(SyntaxNode node, Scope currentScope)
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
                    return Evaluate(s, currentScope);
                case ListNode l:
                    return Evaluate(l, currentScope);
                case RangeNode r:
                    return Evaluate(r, currentScope);
                default:
                    throw new NotImplementedException();
            }
        }
        
        private static List<IValue> Evaluate(IdentifierNode identifier, Scope currentScope = null)
        {
            currentScope = currentScope ?? Global.Scope;

            return Value.ListWith(currentScope[identifier.Name]);
        }

        private static List<IValue> Evaluate(NumericNode numeric)
        {
            return Value.ListWith((Number) numeric.Value);
        }

        private static List<IValue> Evaluate(BooleanNode boolean)
        {
            return Value.ListWith(boolean.Value ? Bool.True : Bool.False);
        }

        private static List<IValue> Evaluate(RangeNode rangeNode, Scope currentScope = null)
        {
            currentScope = currentScope ?? Global.Scope;
            var arguments = rangeNode.Children.Select(c => c.Eval(currentScope));
            return Value.ListWith(StdLib.Range.RangeFromArguments(arguments.ToList(), currentScope));
        }

        private static List<IValue> Evaluate(StringNode str, Scope currentScope = null)
        {
            return Value.ListWith(new UString(str.Value(currentScope)));
        }

        private static List<IValue> GetEvaluatedArgumentsForNamedParameters(IFunction function, ListNode list, Scope currentScope)
        {
            List<IValue> evaluatedArguments = new List<IValue>();
            for (int i = 0; i < function.ArgumentNames.Count; ++i)
            {
                evaluatedArguments.Add(Value.Nil);
            }
            
            //TODO error handling
            string name = null;
            IValue namedValue = null;
            int positionalIndex = 0;
            int namedIndex = 0;
            foreach (var child in list.Children.Skip(1))
            {
                if (child is IdentifierNode id && id.Name.EndsWith(":"))
                {
                    name = id.Name.Substring(0, id.Name.Length - 1);
                }
                else if (name != null)
                {
                    namedValue = Interpret(child, currentScope).First();
                }
                else
                {
                    if (namedIndex > 0) throw new Exception("named parameters must come after positional parameters");
                    evaluatedArguments[positionalIndex] = Interpret(child, currentScope).First();
                    positionalIndex++;
                }

                if (name != null && namedValue != null)
                {
                    int index = function.ArgumentNames.IndexOf(name);
                    if (index != -1)
                    {
                        if (!ReferenceEquals(evaluatedArguments[index], Value.Nil))
                        {
                            throw new Exception($"parameter {name} was already bound");
                        }
                        evaluatedArguments[index] = namedValue;
                        namedIndex++;
                    }
                    name = null;
                    namedValue = null;
                }
            }
            
            return evaluatedArguments;
        }

        private static void PushToCallStack(ListNode list, IFunction function, IValue firstValue, int line)
        {
            if (firstValue is IFunction ifunc) function = ifunc;

            CallStack.Push(new StackInformation
            {
                FileName = function.FileName,
                FunctionName = function.Name,
                Line = function.Line,
                CallingFile = list.File,
                CallingLine = line
            });
        }

        private enum WulFunctionType
        {
            Function,
            Magic,
            Macro,
            NotAFunction
        }

        private static WulFunctionType GetFunctionType(IValue val)
        {
            if (val == null) return WulFunctionType.NotAFunction;
            if (val.MetaType?.Invoke?.IsDefined ?? false)  return WulFunctionType.Function;
            if (val.MetaType?.InvokeMagic?.IsDefined ?? false) return WulFunctionType.Magic;
            if (val.MetaType?.ApplyMacro?.IsDefined ?? false) return WulFunctionType.Macro;
            return WulFunctionType.NotAFunction;
        }

        private static List<IValue> EvaluateFunction(IValue firstValue, ListNode list, Scope currentScope)
        {
            List<IValue> evaluatedList;
            if (list.NamedParameterList)
            {
                evaluatedList = GetEvaluatedArgumentsForNamedParameters((IFunction) firstValue, list, currentScope);
            }
            else
            {
                evaluatedList = list.Children
                    .Skip(1)
                    .SelectMany(node => Interpret(node, currentScope))
                    .Where(v => v != null)
                    .ToList();
            }

            var function = firstValue.MetaType.Invoke.Method;
            PushToCallStack(list, function, firstValue, list.Line);

            var finalList = new List<IValue> { firstValue };
            finalList.AddRange(evaluatedList);

            return function.Evaluate(finalList, currentScope);
        }

        private static List<IValue> EvaluateMagic(IValue firstValue, ListNode list, Scope currentScope)
        {
            var function = firstValue.MetaType.InvokeMagic;
            PushToCallStack(list, function.Method, firstValue, list.Line);
            return function.Invoke(new List<IValue> { firstValue, list }, currentScope);
        }

        private static List<IValue> EvaluateMacro(IValue firstValue, ListNode list, Scope currentScope)
        {
            var function = firstValue.MetaType.ApplyMacro;
            PushToCallStack(list, function.Method, firstValue, list.Line);

            firstValue = function.Invoke(new List<IValue> { firstValue, list }, currentScope).FirstOrDefault();

            //TODO how to avoid the ToSyntaxNode step? ListNodes are evaluated to ListTable
            SyntaxNode node = firstValue as SyntaxNode ?? firstValue.ToSyntaxNode(list.Parent);

            if (node == null) return Value.ListWith(firstValue);
            System.Diagnostics.Debug.WriteLine($"macro -> {node}");
            return Interpret(node, currentScope) ?? Value.EmptyList;
        }

        private static List<IValue> Evaluate(ListNode list, Scope currentScope = null)
        {
            currentScope = currentScope ?? Global.Scope;

            if (list.Children.Count == 0) return Value.ListWith(ListTable.EmptyList);

            var first = list.Children.First();
            List<IValue> value = Value.EmptyList;

            value = Interpret(first, currentScope);

            var firstValue = value.FirstOrDefault();
            var funcType = GetFunctionType(firstValue);
            switch (funcType)
            {
                case WulFunctionType.Function:
                    value = EvaluateFunction(firstValue, list, currentScope);
                    break;
                case WulFunctionType.Magic:
                    value = EvaluateMagic(firstValue, list, currentScope);
                    break;
                case WulFunctionType.Macro:
                    value = EvaluateMacro(firstValue, list, currentScope);
                    break;
                default:
                    //Evaluate the remainder of the list
                    List<IValue> remaining = new List<IValue>(list.Children.Count)
                    {
                        value.First()
                    };

                    foreach (var node in list.Children.Skip(1))
                    {
                        remaining.Add(Interpret(node, currentScope).First());
                    }

                    if (remaining.Count > 0) value = Value.ListWith(new ListTable(remaining));
                    break;
            }
            
            CallStack.Clear();
            return value;
        }
    }
}
