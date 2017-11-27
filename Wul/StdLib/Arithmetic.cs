using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Arithmetic
    {
        [GlobalName("+")]
        internal static IFunction Add = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Add.Invoke(list, scope);
        }, "+");

        [GlobalName("-")]
        internal static IFunction Subtract = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Subtract.Invoke(list, scope);
        }, "-");

        [GlobalName("*")]
        internal static IFunction Multiply = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Multiply.Invoke(list, scope);
        }, "*");

        [GlobalName("/")]
        internal static IFunction Divide = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Divide.Invoke(list, scope);
        }, "/");

        [GlobalName("%")]
        internal static IFunction Modulus = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Modulus.Invoke(list, scope);
        }, "%");

        [GlobalName("**")]
        internal static IFunction Power = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Power.Invoke(list, scope);
        }, "**");
    }
}
