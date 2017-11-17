using System.Collections.Generic;

namespace Wul.Interpreter
{
    class Scope
    {
        public Scope Parent;
        private readonly Dictionary<string, IValue> BoundVariables;

        public Scope(Scope parent = null)
        {
            Parent = parent;
            BoundVariables = new Dictionary<string, IValue>();
        }

        public IValue Get(string key)
        {
            BoundVariables.TryGetValue(key, out IValue val);
            return val ?? Parent?.Get(key) ?? Value.Nil;
        }

        public Scope EmptyChildScope()
        {
            return new Scope(this);
        }

        public void Assign(string key, IValue value)
        {
            if (value == Value.Nil)
            {
                BoundVariables.Remove(key);
            }
            else
            {
                BoundVariables[key] = value;
            }
        }

        public IValue this[string key]
        {
            get => Get(key);

            set => Assign(key, value);
        }
    }
}