namespace Wul.Interpreter
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