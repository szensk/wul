using System.Collections.Generic;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    public interface IValue
    {    
        //Meta-type
        MetaType MetaType { get; set; }

        WulType Type { get; }

        SyntaxNode ToSyntaxNode(SyntaxNode parent);

        string AsString();

        object ToObject();
    }

    public static class IValueHelpers
    {
        public static T Convert<T>(this IValue val) where T : class, IValue
        {
            return Value.Convert<T>(val);
        }

        public static IEnumerable<T> Convert<T>(this IEnumerable<IValue> enumerable) where T : class, IValue
        {
            foreach (IValue value in enumerable)
            {
                yield return Value.Convert<T>(value);
            }
        }
    }
}
