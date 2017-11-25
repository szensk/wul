using System;
using System.Collections.Generic;
using System.Linq;

namespace Wul.Interpreter
{
    class MapTable : IValue
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

        public IValue Get(IValue key)
        {
            _map.TryGetValue(key, out IValue val);
            return val ?? Value.Nil;
        }

        public void Add(IValue key, IValue value)
        {
            _map.Add(key, value);
        }

        protected void Remove(IValue key)
        {
            _map.Remove(key);
        }

        public void Assign(IValue key, IValue value)
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

        public Number Count => _map.Count;

        public string AsString()
        {
            //TODO call as string metamethod
            return "(" + string.Join(", ", _map.Select(s => $"{s.Key.AsString()}:{s.Value.AsString()}").ToList()) + ")";
        }

        public object ToObject()
        {
            return _map;
        }

        private static readonly MapMetaType metaType = new MapMetaType();
        public MetaType MetaType => metaType;

        public IValue this[IValue key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}