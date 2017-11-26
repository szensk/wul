using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    class ListTable : IValue
    {
        private readonly List<IValue> _list;

        public ListTable()
        {
            _list = new List<IValue>();
            ValueMetaType = metaType;
        }

        public ListTable(IValue[] array)
        {
            _list = array.ToList();
            ValueMetaType = metaType;
        }

        public ListTable(IEnumerable<IValue> enumerable)
        {
            _list = enumerable.ToList();
            ValueMetaType = metaType;
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
            if (value == Value.Nil)
            {
                Remove(key);
            }
            else
            {
                _list[(Number) key] = value;
            }
        }

        public Number Count => _list.Count;

        public string AsString()
        {
            //TODO call as string metamethod
            return "(" + string.Join(", ", _list.Select(s => s.AsString()).ToList()) + ")";
        }

        public object ToObject()
        {
            //TODO Not ideal
            return _list.Select(i => i.ToObject()).ToArray();
        }

        private static readonly ListMetaType metaType = new ListMetaType();
        public MetaType ValueMetaType { get; set; }
        public MetaType MetaType => ValueMetaType;

        public IValue this[IValue key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}