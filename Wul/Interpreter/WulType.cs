using System;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;
using Wul.Parser;

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
            Metatype = TypeMetaType.Instance;
        }

        //MetaType of WulType itself
        public MetaType Metatype { get; set; }

        public abstract MetaType DefaultMetaType { get; }

        public WulType Type => null;

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new IdentifierNode(parent, Name);
        }

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
