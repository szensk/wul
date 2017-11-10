namespace Wul.Interpreter
{
    abstract class Value : IValue
    {
        //GetMetatable
        
        //SetMetatable

        public static Value Nil = new Nill();
    }

    class Nill : Value
    {
        
    }
}