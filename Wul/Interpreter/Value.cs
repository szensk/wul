namespace Wul.Interpreter
{
    abstract class Value : IValue
    {
        //GetMetatable
        
        //SetMetatable
        public abstract string AsString();

        public static Value Nil = new Nill();
    }

    internal class Nill : Value
    {
        public override string AsString()
        {
            return "nil";
        }
    }
}