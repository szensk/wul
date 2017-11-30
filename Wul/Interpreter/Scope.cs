using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser;
using Wul.StdLib;

namespace Wul.Interpreter
{
    public class Scope
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

        public void Remove(string key)
        {
            BoundVariables.Remove(key);
        }

        //This actually declares a new binding
        public void Declare(string key, IValue value)
        {
            BoundVariables[key] = value;
        }

        public void Set(string key, IValue value)
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
                s.BoundVariables[key] = value;
            }
        }

        public Scope CloseScope(ListNode body)
        {
            var identifierNodes = body.IdentifierNodes();
            string[] magicVariables = {"...", "self"};
            var referencedNames = identifierNodes.Select(i => i.Name).Concat(magicVariables);

            Scope closedScope = new Scope(Global.Scope);
            foreach (string name in referencedNames)
            {
                //TODO should it only include bound variables?
                closedScope[name] = this[name];
            }

            return closedScope;
        }

        public IValue this[string key]
        {
            get => Get(key);

            set => Declare(key, value);
        }
    }
}