using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class SyntaxNodeMetaType : MetaType
    {
        public static readonly SyntaxNodeMetaType Instance = new SyntaxNodeMetaType();

        private SyntaxNodeMetaType()
        {
            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(IdentityType, Type.Name);
        }
    }
}