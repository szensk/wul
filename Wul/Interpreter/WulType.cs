using System;
using System.Runtime.CompilerServices;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.Interpreter
{
    public abstract class WulType : IValue
    {
        public Type RawType { get;  }
        public string Name { get; }
        public string FileName { get; }
        public int Line { get; }

        protected WulType(
            string name, 
            Type type, 
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            Name = name;
            RawType = type;
            Line = line;
            FileName = $"{System.IO.Path.GetFileName(file)} {member}";
            MetaType = TypeMetaType.Instance;
        }

        //MetaType of WulType itself
        public MetaType MetaType { get; set; }

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
