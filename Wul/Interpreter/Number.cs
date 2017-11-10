using System;

namespace Wul.Interpreter
{
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

        // Constructors
        private Number(int i)
        {
            
        }

        private Number(double d)
        {
            
        }

        // Conversions
        public static implicit operator int(Number i)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Number(int i)
        {
            return i < 256 ? SmallNumberCache[i] : new Number(i);
        }

        // Comparisons
    }
}