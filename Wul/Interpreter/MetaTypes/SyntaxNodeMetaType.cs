using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class SyntaxNodeMetaType : MetaType
    {
        public static readonly SyntaxNodeMetaType Instance = new SyntaxNodeMetaType();

        private SyntaxNodeMetaType()
        {
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
            Invoke.Method = new NetFunction(IdentityList, Invoke.Name);
        }
    }
}