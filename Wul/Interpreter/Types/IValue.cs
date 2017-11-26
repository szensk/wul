namespace Wul.Interpreter.Types
{
    public interface IValue
    {    
        //Meta-type
        MetaType MetaType { get; set; }

        WulType Type { get; }

        string AsString();

        object ToObject();
    }
}
