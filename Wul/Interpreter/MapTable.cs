using System;
using System.Collections.Generic;

namespace Wul.Interpreter
{
    class MapTable : Table
    {
        private readonly Dictionary<IValue, IValue> _map;

        public MapTable()
        {
            _map = new Dictionary<IValue, IValue>();
        }

        public MapTable(ListTable list)
        {
            _map = new Dictionary<IValue, IValue>();
            for (Number i = 0; i < list.Count; i++)
            {
                _map.Add(i, list[i]);
            }
        }

        public MapTable(object o)
        {
            throw new NotImplementedException();
        }

        public override IValue Get(IValue key)
        {
            _map.TryGetValue(key, out IValue val);
            return val ?? Value.Nil;
        }

        public override void Add(IValue key, IValue value)
        {
            _map.Add(key, value);
        }

        protected override void Remove(IValue key)
        {
            _map.Remove(key);
        }

        public override void Assign(IValue key, IValue value)
        {
            if (value == Value.Nil)
            {
                Remove(key);
            }
            else
            {
                _map[key] = value;
            }
        }

        public override Number Count => _map.Count;

        public override IValue this[IValue key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}