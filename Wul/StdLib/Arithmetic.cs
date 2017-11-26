using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class Arithmetic
    {
        internal static IFunction Add = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Add.Invoke(list, scope);
        }, "+");

        internal static IFunction Subtract = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Subtract.Invoke(list, scope);
        }, "-");

        internal static IFunction Multiply = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Multiply.Invoke(list, scope);
        }, "*");

        internal static IFunction Divide = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Divide.Invoke(list, scope);
        }, "/");

        internal static IFunction Modulus = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Modulus.Invoke(list, scope);
        }, "%");

        internal static IFunction Power = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Power.Invoke(list, scope);
        }, "**");
    }
}
