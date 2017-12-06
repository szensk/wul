using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class SyntaxNodeMetaType : MetaType
    {
        public static readonly SyntaxNodeMetaType Instance = new SyntaxNodeMetaType();

        private SyntaxNodeMetaType() : base(null)
        {
            AsString = new NetFunction(IdentityString, AsString.Name);
            Type = new NetFunction(IdentityType, Type.Name);

            InitializeDictionary();
        }
    }
}