using System;
using System.Collections;
using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class IEnumerableConverter : ValueConverter<IEnumerable>
    {
        //TODO return lazy enumeration?
        public override IValue ConvertToIValue(IEnumerable original)
        {
            if (original == null) return Value.Nil;
            var values = new ListTable();
            foreach (var value in original)
            {
                values.Add(Framework.ConvertToIValue(value));
            }
            return values;
        }

        //TODO iterate over any enumerable
        public override IEnumerable ConvertFromIValue(IValue original)
        {
            if (original == Value.Nil) return null;
            if (original is ListTable lt)
            {
                return lt.AsList();
            }
            throw new Exception($"Unable to convert {original.Type.Name} to IEnumerable");
        }
    }
}
