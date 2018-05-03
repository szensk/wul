using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    // proxy class for exposing .net properties to a scope
    public class NetProperty<T> : IValue where T : class, IValue
    {
        public T Value { get; private set; }

        private NetProperty(T value)
        {
            Value = value;
            MetaType = value.MetaType.Clone();
            MetaType.Set.Method = NetFunction.FromSingle(ProxySet, MetaType.Set.Name);
        }

        private IValue ProxySet(List<IValue> list, Scope s)
        {
            if (list.Count < 2) throw new Exception("Not enough arguments, expected at least 2");
            var first = list[1];
            if (first.GetType() != typeof(T)) throw new Exception($"Unable to set property of type {typeof(T)} to {first.Type}");
            Value = (T)first;
            return first;
        }

        public static implicit operator T(NetProperty<T> np)
        {
            return np.Value;
        }

        public static implicit operator NetProperty<T>(T b)
        {
            return new NetProperty<T>(b);
        }

        public MetaType MetaType { get; set; }
        public WulType Type => Value.Type;
        public SyntaxNode ToSyntaxNode(SyntaxNode parent) => Value.ToSyntaxNode(parent);
        public string AsString() => Value.AsString();
        public object ToObject() => Value.ToObject();
    }
}
