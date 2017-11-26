using System;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;

namespace Wul.Interpreter
{
    public abstract class WulType : IValue
    {
        public Type RawType { get;  }
        public string Name { get; }

        protected WulType(string name, Type type)
        {
            Name = name;
            RawType = type;
            MetaType = TypeMetaType.Instance;
        }

        public MetaType MetaType { get; set; }

        public WulType Type => null;

        public string AsString()
        {
            return Name;
        }

        public object ToObject()
        {
            throw new NotImplementedException();
        }
    }
}
