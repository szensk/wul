namespace Wul.Interpreter.Types
{
    public interface IValue
    {
        //All metamethods go here
        MetaType MetaType { get; }

        string AsString();

        object ToObject();
    }
}
