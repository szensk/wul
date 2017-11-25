using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class MapMetaType : MetaType
    {
        //TODO 
        public MapMetaType()
        {
            //AsString
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
        }
    }
}