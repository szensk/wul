using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class SyntaxNodeMetaType : MetaType
    {
        public SyntaxNodeMetaType()
        {
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
        }
    }
}