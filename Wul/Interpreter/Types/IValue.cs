using Wul.Parser;

namespace Wul.Interpreter.Types
{
    public interface IValue
    {    
        //Meta-type
        MetaType Metatype { get; set; }

        WulType Type { get; }

        SyntaxNode ToSyntaxNode(SyntaxNode parent);

        string AsString();

        object ToObject();
    }
}
