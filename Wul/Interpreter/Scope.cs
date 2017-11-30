using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.Interpreter
{
    internal class Binding
    {
        public Binding(IValue value)
        {
            Value = value;
        }

        public IValue Value { get; set; }
    }

    public class Scope
    {
        public Scope Parent;
        private readonly Dictionary<string, Binding> BoundVariables;

        public Scope(Scope parent = null)
        {
            Parent = parent;
            BoundVariables = new Dictionary<string, Binding>();
        }

        public Scope EmptyChildScope()
        {
            return new Scope(this);
        }

        public IValue Get(string key)
        {
            BoundVariables.TryGetValue(key, out Binding val);
            return val?.Value ?? Parent?.Get(key) ?? Value.Nil;
        }

        private Binding GetBinding(string key)
        {
            BoundVariables.TryGetValue(key, out Binding val);
            return val ?? Parent?.GetBinding(key) ?? null;

        }

        public void Remove(string key)
        {
            BoundVariables.Remove(key);
        }

        //This actually declares a new binding
        public void Set(string key, IValue value)
        {
            if (BoundVariables.TryGetValue(key, out Binding val))
            {
                val.Value = value;
            }
            else
            {
                BoundVariables[key] = new Binding(value);
            }
        }

        public void Assign(string key, IValue value)
        {
            Scope s = this;
            while (s != null && !s.BoundVariables.ContainsKey(key))
            {
                s = s.Parent;
            }

            if (s == null)
            {
                throw new Exception($"upval {key} does not exist");
            }
            else
            {
                if (ReferenceEquals(value, Value.Nil))
                {
                    s.BoundVariables.Remove(key);
                }
                else
                {
                    s.BoundVariables[key].Value = value;
                }
            }
        }

        public Scope CloseScope(ListNode body)
        {
            var identifierNodes = body.IdentifierNodes();
            var referencedNames = identifierNodes.Select(i => i.Name).ToHashSet();

            Scope closedScope = new Scope();

            //Construct a new scope with referenced bindings
            foreach (string name in referencedNames)
            {
                Binding binding = GetBinding(name);
                if (binding != null)
                {
                    closedScope.BoundVariables.Add(name, binding);
                }
            }

            return closedScope;
        }

        public Scope CompletelyCloseScope()
        {
            Scope closedScope = new Scope();

            Scope currentScope = this;
            while (currentScope != null)
            {
                foreach (var binding in currentScope.BoundVariables)
                {
                    closedScope.BoundVariables.TryAdd(binding.Key, binding.Value);
                }
                currentScope = currentScope.Parent;
            }

            return closedScope;
        }

        public IValue this[string key]
        {
            get => Get(key);

            set => Set(key, value);
        }
    }
}