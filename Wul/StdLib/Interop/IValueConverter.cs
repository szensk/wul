using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    interface IValueConverter<TNetValue, TWulValue>
    {
        TWulValue ConvertToIValue(TNetValue original);
        TNetValue ConvertFromIValue(TWulValue original);
    }

    interface IValueConverter<TNetValue> : IValueConverter<TNetValue, IValue>
    {

    }
}