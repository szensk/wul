using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    public class RangeType : WulType
    {
        public RangeType() : base("Range", typeof(Range))
        {
        }

        public static readonly RangeType Instance = new RangeType();
        public override MetaType DefaultMetaType { get; }
    }

    //TODO hashcode, equals
    public class Range : IValue
    {
        private readonly double _Start;
        private readonly double? _End;
        private readonly double? _Increment;

        public Range(double start, double? end = null, double? increment = null)
        {
            _Start = start;
            _End = end;
            _Increment = increment;
        }

        public MetaType MetaType { get; set; } = RangeMetaType.Instance;
        public WulType Type => RangeType.Instance;

        public ListTable AsList()
        {
            ListTable list = new ListTable();

            var actualList = list.AsList();

            actualList.Add(First);

            Range rem = Remainder;
            while (rem != null)
            {
                actualList.Add(rem.First);
                rem = rem.Remainder;
            }

            return list;
        }

        public Number First => _Start;

        public Range Remainder
        {
            get
            {
                if (_Increment.HasValue && (_Increment > 0 && _Start < _End || _Increment < 0 && _Start > _End) || !_End.HasValue)
                {
                    return new Range(_Start + _Increment.Value, _End, _Increment);
                }
                else
                {
                    return null;
                }
            }
        }

        public Bool Contains(Number n)
        {
            bool result = n.Value >= _Start && n.Value <= _End && _Increment > 0 ||
                          n.Value <= _Start && n.Value >= _End && _Increment < 0;
            return result ? Bool.True : Bool.False;
        }
        
        //TODO Set operations of ranges e.g. Union, Intersection, IsSubset

        public Number Count 
        {
            get
            {
                if (!_Increment.HasValue)
                {
                    return 1;
                }
                if (!_End.HasValue)
                {
                    double result = _Increment > 0 ? double.PositiveInfinity : double.NegativeInfinity;
                    Number numberResult = (Number) result;
                    return numberResult;
                }
                return (int)((_End - _Start) / _Increment) + 1;
            }
        }

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            List<SyntaxNode> children = new List<SyntaxNode>();
            children.Add(((Number)_Start).ToSyntaxNode(parent));
            if (_End.HasValue)
            {
                children.Add(((Number) _End.Value).ToSyntaxNode(parent));
            }
            else
            {
                children.Add(Value.Nil.ToSyntaxNode(parent));
            }
            children.Add(((Number)_Increment.Value).ToSyntaxNode(parent));
           return new RangeNode(parent, children);
        }

        public string AsString()
        {
            return $"Range[{_Start} {_End} by {_Increment}]";
        }

        //Return an enumerator
        public object ToObject()
        {
            throw new NotImplementedException();
        }
    }
}
