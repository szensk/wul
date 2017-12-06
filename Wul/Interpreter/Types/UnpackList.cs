using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    public class UnpackList : IValue
    {
        public List<IValue> Values { get; }
        public MetaType Metatype { get; set; }
        public WulType Type => null;

        public UnpackList(ListTable list)
        {
            Values = list.AsList();
        }

        public UnpackList(IEnumerable<IValue> values)
        {
            Values = values.ToList();
        }

        public static List<IValue> Replace(List<IValue> list)
        {
            List<IValue> replacements = new List<IValue>();
            foreach (var value in list)
            {
                if (value is UnpackList unpackList)
                {
                    replacements.AddRange(Replace(unpackList.Values));
                }
                else
                {
                    replacements.Add(value);
                }
            }
            return replacements;
        }

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            throw new NotImplementedException();
        }

        public object ToObject()
        {
            throw new NotImplementedException();
        }
    }
}
