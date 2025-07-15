using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    interface IValueConverter<TNetValue>
    {
        IValue ConvertToIValue(TNetValue original);
        TNetValue ConvertFromIValue(IValue original);
        int Priority { get; }
    }

    interface IValueConverter
    {
        IValue ConvertToIValue(object original);
        object ConvertFromIValue(IValue original);
        int Priority { get; }

    }

    abstract class ValueConverter<TNetValue> : IValueConverter<TNetValue>, IValueConverter
    {
        public abstract IValue ConvertToIValue(TNetValue original);

        public abstract TNetValue ConvertFromIValue(IValue original);

        public IValue ConvertToIValue(object original)
        {
            return ConvertToIValue((TNetValue) original);
        }

        object IValueConverter.ConvertFromIValue(IValue original)
        {
            return ConvertFromIValue(original);
        }

        public virtual int Priority => 1;
    }
}