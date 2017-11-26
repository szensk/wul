using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    public class MapType : WulType
    {
        public MapType() : base("Map", typeof(MapType))
        {
        }

        public static readonly MapType Instance = new MapType();
        public override MetaType DefaultMetaType => MapMetaType.Instance;
    }

    class MapTable : IValue
    {
        private readonly Dictionary<IValue, IValue> _map;

        public MapTable()
        {
            _map = new Dictionary<IValue, IValue>();
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

        public WulType Type => MapType.Instance;

        public string AsString()
        {
            //TODO call as string metamethod
            return "(" + string.Join(", ", _map.Select(s => $"{s.Key.AsString()}:{s.Value.AsString()}").ToList()) + ")";
        }

        public object ToObject()
        {
            return _map;
        }

        public MetaType MetaType { get; set; }

        public IValue this[IValue key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}