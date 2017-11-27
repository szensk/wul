using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    public class MapType : WulType
    {
        public MapType() : base("Map", typeof(MapTable))
        {
        }

        public static readonly MapType Instance = new MapType();
        public override MetaType DefaultMetaType => MapMetaType.Instance;
    }

    class MapTable : IValue
    {
        private readonly IDictionary<IValue, IValue> _map;

        public MapTable()
        {
            _map = new Dictionary<IValue, IValue>();
            MetaType = MapMetaType.Instance;
        }

        public MapTable(IDictionary<IValue, IValue> dict)
        {
            _map = dict;
            MetaType = MapMetaType.Instance;
        }

        public MapTable(ListTable list)
        {
            _map = new Dictionary<IValue, IValue>();
            for (Number i = 0; i < list.Count; i++)
            {
                _map.Add(i, list[i]);
            }
            MetaType = MapMetaType.Instance;
        }

        public MapTable(object o)
        {
            MetaType = MapMetaType.Instance;

            //TODO iterate through each field and add to map
            //for each property generate a NetFunction

            throw new NotImplementedException();
        }

        public IDictionary<IValue, IValue> AsDictionary()
        {
            return _map;
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

        public WulType Type => MapType.Instance;

        public string AsString()
        {
            return "(" + string.Join(", ", _map.Select(s => $"{s.Key.AsString()}:{s.Value.AsString()}").ToList()) + ")";
        }

        public object ToObject()
        {
            Dictionary<object, object> objectMap = new Dictionary<object, object>();

            foreach (var kvp in _map)
            {
                var key = kvp.Key.ToObject();
                var val = kvp.Value.ToObject();
                objectMap[key] = val;
            }

            return objectMap;
        }

        public MetaType MetaType { get; set; }

        public IValue this[IValue key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}