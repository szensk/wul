using System;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class ArrayConverter : IValueConverter<object[]>
    {
        public IValue ConvertToIValue(object[] original)
        {
            if (original == null) return Value.Nil;
            return new ListTable(original.Select(Framework.ConvertToIValue));
        }

        public object[] ConvertFromIValue(IValue original)
        {
            if (original == Value.Nil) return null;
            if (original is ListTable)
            {
                return (object[]) original.ToObject();
            }
            throw new Exception($"Unable to convert {original.Type.Name} to Array");
        }
    }
}