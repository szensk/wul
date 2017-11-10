using System.Collections.Generic;
using System.Linq;

namespace Wul.Interpreter
{
    class ListTable : Table
    {
        private List<IValue> _list;

        public ListTable()
        {
            _list = new List<IValue>();
        }

        public ListTable(IValue[] array)
        {
            _list = array.ToList();
        }

        public override IValue Get(IValue key)
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

        public override void Add(IValue key, IValue value)
        {
            //TODO check if need to convert to MapTable
            _list.Add(value);
        }

        protected override void Remove(IValue key)
        {
            //TODO check if need to convert to MapTable
            _list.RemoveAt((Number)key);
        }

        public override void Assign(IValue key, IValue value)
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

        public override Number Count => _list.Count;

        public override IValue this[IValue key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}