namespace Wul.Interpreter
{
    public interface IValue
    {
        //All metamethods go here
        MetaType MetaType { get; }

        string AsString();
    }
}
