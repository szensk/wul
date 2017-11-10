namespace Wul.Interpreter
{
    interface IExpression<T>
    {
        T Evaluate();
    }
}
