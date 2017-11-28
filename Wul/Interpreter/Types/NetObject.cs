using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wul.Interpreter.MetaTypes;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    class NetObjectType : WulType
    {
        public NetObjectType(Type type) : base(type.Name, type)
        {
            
        }

        private static readonly Dictionary<Type, NetObjectType> typeMap = new Dictionary<Type, NetObjectType>();

        public static NetObjectType GetTypeForObject(object o)
        {
            Type t = o.GetType();
            if (typeMap.TryGetValue(t, out NetObjectType objectType))
            {
                return objectType;
            }
            else
            {
                objectType = new NetObjectType(t);
                typeMap.Add(t, objectType);
                return objectType;
            }
            
        }

        public override MetaType DefaultMetaType => null;
    }

    public class NetObject : IValue
    {
        private readonly object Value;
        private readonly Type ValueType;

        public NetObject(object o)
        {
            Value = o;
            ValueType = o.GetType();
            MetaType = NetObjectMetaType.Instance;
        }

        public IValue Get(string name)
        {
            FieldInfo field = ValueType.GetField(name);

            if (field != null)
            {
                return new NetObject(field.GetValue(Value));
            }

            PropertyInfo property = ValueType.GetProperty(name);

            if (property != null)
            {
                return new NetObject(property.GetMethod.Invoke(Value, null));
            }

            throw new Exception($"{Value} has no field/property {name}");
        }

        public void Set(string name, IValue value)
        {
            FieldInfo field = ValueType.GetField(name);

            if (field != null)
            {
                field.SetValue(Value, value.ToObject());
                return;
            }

            PropertyInfo property = ValueType.GetProperty(name);

            if (property != null)
            {
                MethodInfo setMethod = property.SetMethod;
                if (setMethod == null) throw new Exception($"Property {name} has no setter");
                setMethod.Invoke(Value, new [] {value.ToObject()});
                return;
            }

            throw new Exception($"{Value} has no field/property {name}");
        }

        public IValue Call(string name, params IValue[] arguments)
        {
            var values = arguments.Select(a => a.ToObject()).ToArray();
            var types = values.Select(v => v.GetType()).ToArray();

            var method = ValueType.GetMethod(name, types);

            if (method != null)
            {
                return new NetObject(method.Invoke(Value, values));
            }

            string argumentTypes = string.Join(", ", types.Select(t => t.Name));
            throw new Exception($"Unable to find method {name}({argumentTypes})");
        }

        public MetaType MetaType { get; set; }

        public WulType Type => NetObjectType.GetTypeForObject(Value);

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return Value.ToString();
        }

        public object ToObject()
        {
            return Value;
        }
    }
}
