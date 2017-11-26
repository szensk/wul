namespace Wul.Interpreter.Types
{
    public interface IValue
    {
        //Meta-type for this value
        MetaType ValueMetaType { get; set; }
        
        //Meta-type
        MetaType MetaType { get; }

        string AsString();

        object ToObject();
    }
}
