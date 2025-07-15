using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.Parser.Parsers;

namespace Wul.StdLib
{
    static class Helpers
    {
        public static IValue Eval(this SyntaxNode node, Scope scope)
        {
            IValue result = WulInterpreter.Interpret(node, scope).FirstOrDefault() ?? Value.Nil;
            while (result is SyntaxNode)
            {
                result = WulInterpreter.Interpret(result as SyntaxNode, scope).FirstOrDefault() ?? Value.Nil;
            }
            return result;
        }

        public static List<IValue> EvalMany(this SyntaxNode node, Scope scope)
        {
            List<IValue> result = WulInterpreter.Interpret(node, scope) ?? new List<IValue>();
            while (result.FirstOrDefault() is SyntaxNode)
            {
                result = WulInterpreter.Interpret(result.FirstOrDefault() as SyntaxNode, scope);
            }
            return result;
        }

        public static List<IValue> EvalManyOnce(this SyntaxNode node, Scope scope)
        {
            return WulInterpreter.Interpret(node, scope);
        }

        public static IValue EvalOnce(this SyntaxNode node, Scope scope)
        {
            return WulInterpreter.Interpret(node, scope).FirstOrDefault() ?? Value.Nil;
        }


        public static List<IValue> PushFront(this List<IValue> list, params IValue[] values)
        {
            list.InsertRange(0, values);
            return list;
        }

        public static List<IValue> PushBack(this List<IValue> list, params IValue[] values)
        {
            list.AddRange(values);
            return list;
        }

        public static ProgramNode Parse(string programText, ProgramParser parser = null)
        {
            parser = parser ?? new ProgramParser();
            return (ProgramNode) parser.Parse(programText);
        }

        // full file name
        public static ProgramNode ParseFile(string fileName)
        {
            ProgramParser parser = new ProgramParser(fileName);
            string contents = File.ReadAllText(fileName);
            return Parse(contents, parser);
        }

        // full file name
        public static List<IValue> LoadFile(string fileName, Scope scope)
        {
            ProgramNode program = ParseFile(fileName);
            return WulInterpreter.Interpret(program, scope);
        }

        public static List<IValue> LoadString(string script, Scope scope)
        {
            ProgramNode program = Parse(script);
            return WulInterpreter.Interpret(program, scope);
        }

        public static WulString ToWulString(IValue value)
        {
            if (ReferenceEquals(value, Value.Nil)) return new WulString("nil");
            IValue val = value.MetaType.AsString.Invoke(Value.ListWith(value), null).First();
            if (val is WulString s) return s;
            throw new Exception("AsString did not return a string");
        }

        public static string ToString(IValue value)
        {
            return ToWulString(value).AsString();
        }

        public static Number ToNumber(IValue value)
        {
            if (ReferenceEquals(value, Value.Nil)) return (Number)double.NaN;
            if (value is Number n) return n;
            if (value is WulString s) return (Number)double.Parse(s.Value);
            throw new Exception("AsNumber did not return a number");
        }

        public static IValue AssertNotNil(this IValue value)
        {
            if (ReferenceEquals(value, Value.Nil)) throw new Exception("nil not expected");
            return value;
        }

        public static IEnumerable<T> IterateOverEnumerable<T>(this IValue enumerable, Func<IValue, T> callback, Scope scope)
        {
            var rem = enumerable.MetaType.Remainder;
            var first = enumerable.MetaType.At;

            if (rem == null || !rem.IsDefined || first == null || !first.IsDefined) throw new Exception("Must be enumerable");

            List<T> result = [];

            while (enumerable != null && enumerable != Value.Nil && enumerable != ListTable.EmptyList && !(enumerable is WulString s && s.Value == string.Empty))
            {
                if (callback != null)
                {
                    var firstElement = first.Invoke(Value.ListWith(enumerable, (Number)0), scope).First();
                    T cbresult = callback(firstElement);
                    result.Add(cbresult);

                }
                enumerable = rem.Invoke(Value.ListWith(enumerable), scope).First();
            }

            return result;
        }

        public static IEnumerable<IValue> IterateOverEnumerable(this IValue enumerable, Scope scope)
        {
            var rem = enumerable.MetaType.Remainder;
            var first = enumerable.MetaType.At;

            if (rem == null || !rem.IsDefined || first == null || !first.IsDefined) throw new Exception("Must be enumerable");

            while (enumerable != null && enumerable != Value.Nil && enumerable != ListTable.EmptyList && !(enumerable is WulString s && s.Value == string.Empty))
            {
                var firstElement = first.Invoke(Value.ListWith(enumerable, (Number)0), scope).First();
                yield return firstElement;
                enumerable = rem.Invoke(Value.ListWith(enumerable), scope).First();
            }

            yield break;
        }
    }
}

