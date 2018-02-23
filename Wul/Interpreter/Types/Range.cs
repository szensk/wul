using System;
using System.Collections.Generic;
using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    public class RangeType : WulType
    {
        public RangeType() : base("Range", typeof(Range))
        {
        }

        public static readonly RangeType Instance = new RangeType();
        public override MetaType DefaultMetaType => RangeMetaType.Instance;
    }

    public class Range : IValue
    {
        private readonly double _start;
        private readonly double? _end;
        private readonly double? _increment;

        public Range(double start, double? end = null, double? increment = null)
        {
            _start = start;
            _end = end;
            _increment = increment;
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

        public Number First => _start;

        public Range Remainder
        {
            get
            {
                if (_increment.HasValue && (_increment > 0 && _start < _end || _increment < 0 && _start > _end) || !_end.HasValue)
                {
                    double inc = _increment ?? 0;
                    return new Range(_start + inc, _end, _increment);
                }
                else
                {
                    return null;
                }
            }
        }

        public Bool Contains(Number n)
        {
            bool result = n.Value >= _start && n.Value <= _end && _increment > 0 ||
                          n.Value <= _start && n.Value >= _end && _increment < 0;
            return result ? Bool.True : Bool.False;
        }
        
        //TODO Set operations of ranges e.g. Union, Intersection, IsSubset

        public Number Count 
        {
            get
            {
                if (!_increment.HasValue)
                {
                    return 1;
                }
                if (!_end.HasValue)
                {
                    double result = _increment > 0 ? double.PositiveInfinity : double.NegativeInfinity;
                    Number numberResult = result;
                    return numberResult;
                }
                return (int)((_end - _start) / _increment) + 1;
            }
        }

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            List<SyntaxNode> children = new List<SyntaxNode>();
            children.Add(((Number)_start).ToSyntaxNode(parent));
            if (_end.HasValue)
            {
                children.Add(((Number) _end.Value).ToSyntaxNode(parent));
            }
            else
            {
                children.Add(Value.Nil.ToSyntaxNode(parent));
            }
            if (_increment.HasValue) children.Add(((Number)_increment.Value).ToSyntaxNode(parent));
            return new RangeNode(parent, children);
        }

        public string AsString()
        {
            return $"Range[{_start} {_end} by {_increment}]";
        }

        //Return an enumerator
        public object ToObject()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            var range = obj as Range;
            return range != null &&
                   _start == range._start &&
                   EqualityComparer<double?>.Default.Equals(_end, range._end) &&
                   EqualityComparer<double?>.Default.Equals(_increment, range._increment);
        }

        public override int GetHashCode()
        {
            var hashCode = 1157971746;
            hashCode = hashCode * -1521134295 + _start.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<double?>.Default.GetHashCode(_end);
            hashCode = hashCode * -1521134295 + EqualityComparer<double?>.Default.GetHashCode(_increment);
            return hashCode;
        }
    }
}
