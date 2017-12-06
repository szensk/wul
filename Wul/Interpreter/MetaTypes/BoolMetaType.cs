using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class BoolMetaType : MetaType
    {
        public static readonly BoolMetaType Instance = new BoolMetaType();

        private BoolMetaType() : base(null)
        {
            //Logical
            Not = new NetFunction(DoNot, Not.Name);

            //TODO Bitwise

            //Equality
            Equal = new NetFunction(IdentityEqual, Equal.Name);

            //String
            AsString = new NetFunction(IdentityString, AsString.Name);
            Type = new NetFunction(IdentityType, Type.Name);

            InitializeDictionary();
        }

        public IValue DoNot(List<IValue> arguments, Scope s)
        {
            var bools = arguments.Select(a => a as Bool).ToList();

            if (bools.Any(b => b == null))
            {
                throw new InvalidOperationException("All must be booleans");
            }

            var notBools = bools.Select(b => b.Value ? Bool.False : Bool.True).ToList();

            if (notBools.Count <= 1)
            {
                return (IValue)notBools.FirstOrDefault() ?? Value.Nil;
            }
            else
            {
                return new ListTable(notBools);
            }
        }

        public IValue DoAnd(List<IValue> arguments, Scope s)
        {
            var bools = arguments.Select(a => a as Bool).ToList();

            if (bools.Any(b => b == null))
            {
                throw new InvalidOperationException("All must be booleans");
            }

            return bools.All(b => b.Value) ? Bool.True : Bool.False;
        }

        public IValue DoOr(List<IValue> arguments, Scope s)
        {
            var bools = arguments.Select(a => a as Bool).ToList();

            if (bools.Any(b => b == null))
            {
                throw new InvalidOperationException("All must be booleans");
            }

            return bools.Any(b => b.Value) ? Bool.True : Bool.False;
        }
    }
}