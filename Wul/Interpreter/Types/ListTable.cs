using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    public class ListType : WulType
    {
        public ListType() : base("List", typeof(ListTable))
        {
        }

        public static readonly ListType Instance = new ListType();
        public override MetaType DefaultMetaType => ListMetaType.Instance;
    }

    public class ListTable : IValue
    {
        private readonly List<IValue> _list;

        public ListTable()
        {
            _list = new List<IValue>();
            MetaType = ListMetaType.Instance;
        }

        public ListTable(IValue[] array)
        {
            _list = array.ToList();
            MetaType = ListMetaType.Instance;
        }

        public ListTable(IEnumerable<IValue> enumerable)
        {
            _list = enumerable.ToList();
            MetaType = ListMetaType.Instance;
        }

        public List<IValue> AsList()
        {
            return _list;
        }

        public IValue Get(IValue key)
        {
            int index = (Number) key;
            if (index >= Count || index < 0)
            {
                return Value.Nil;
            }
            else
            {
                return _list[index];
            }
        }

        public void Add(IValue key, IValue value)
        {
            _list.Add(value);
        }

        protected void Remove(IValue key)
        {
            _list.RemoveAt((Number)key);
        }

        public void Assign(IValue key, IValue value)
        {
            if (ReferenceEquals(value, Value.Nil))
            {
                Remove(key);
            }
            else
            {
                int index = (Number) key;
                while (index >= _list.Count)
                {
                    _list.Add(Value.Nil);
                }
                _list[index] = value;
            }
        }

        public Number Count => _list.Count;

        public WulType Type => ListType.Instance;

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new ListNode(parent, _list.Select(i => i.ToSyntaxNode(parent)).ToList());
        }

        public string AsString()
        {
            return "(" + string.Join(", ", _list.Select(s => s.AsString()).ToList()) + ")";
        }

        public object ToObject()
        {
            return _list.Select(i => i.ToObject()).ToArray();
        }

        public MetaType MetaType { get; set; }

        public IValue this[IValue key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}