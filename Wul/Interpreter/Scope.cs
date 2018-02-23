using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.Interpreter
{
    public class Binding
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
        public Dictionary<string, Binding> BoundVariables { get; }
        public List<string> Usings { get; private set; }

        public Scope(Scope parent = null)
        {
            Parent = parent;
            BoundVariables = new Dictionary<string, Binding>();
            Usings = parent?.Usings.Select(s=>s).ToList() ?? new List<string>();
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

        //Overwrites current binding but only in this scope
        public void BindLocal(string key, IValue value)
        {
            BoundVariables[key] = new Binding(value);
        }

        //If you assign the nil value, it removes the binding
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

        //A closed scope binds variables at the point of definition
        //construct a new closed scope, only referenced bindings
        public Scope CloseScope(ListNode body)
        {
            var identifierNodes = body.IdentifierNodes();
            var referencedNames = identifierNodes.Select(i => i.Name).ToHashSet();

            Scope closedScope = new Scope { Usings = Usings.ToList() };

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

        //constructs a new closed scope, including unreferenced bindings
        public Scope CompletelyCloseScope()
        {
            Scope closedScope = new Scope { Usings = Usings.ToList() };

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