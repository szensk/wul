using System;
using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    public class NumberType : WulType
    {
        public NumberType() : base("Number", typeof(Number))
        {
        }

        public static readonly NumberType Instance = new NumberType();
    }

    class Number : IValue
    {
        #region Static Methods
        private static readonly Number[] SmallNumberCache;

        static Number()
        {
            SmallNumberCache = new Number[256];
            for (int i = 0; i < SmallNumberCache.Length; i++)
            {
                SmallNumberCache[i] = new Number(i);
            }
        }
        #endregion Static Methods

        public readonly double Value;

        // Constructors
        private Number(int i)
        {
            Value = (double) i;
            MetaType = metaType;
        }

        private Number(double d)
        {
            Value = d;
            MetaType = metaType;
        }

        // Conversions
        public static implicit operator int(Number i)
        {
            return (int) i.Value;
        }

        public static implicit operator Number(int i)
        {
            return i < 256 && i >= 0 ? SmallNumberCache[i] : new Number(i);
        }

        public static implicit operator Number(double d)
        {
            if (Math.Floor(d) == d && d < 256 && d > -0.1)
            {
                return SmallNumberCache[(int) d];
            }
            else
            {
                return new Number(d);
            }
        }

        public WulType Type => NumberType.Instance;

        public string AsString()
        {
            return $"{Value}";
        }

        public object ToObject()
        {
            return Value;
        }

        //TODO do the same for other types
        private static readonly NumberMetaType metaType = new NumberMetaType();
        public MetaType MetaType { get; set; }
    }
}