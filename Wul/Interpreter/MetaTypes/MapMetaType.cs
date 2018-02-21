using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser;
using Wul.StdLib;

namespace Wul.Interpreter.MetaTypes
{
    public class MapMetaType : MetaType
    {
        public static readonly MapMetaType Instance = new MapMetaType();

        private MapMetaType()
        {
            //Equality
            Equal.Method = NetFunction.FromSingle(AreEqual, Equal.Name);

            //List
            At.Method = NetFunction.FromSingle(AtKey, At.Name);
            Set.Method = NetFunction.FromSingle(SetKey, Set.Name);
            Count.Method = NetFunction.FromSingle(Length, Count.Name);

            //Other
            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(IdentityType, Type.Name);
        }

        private IValue AtKey(List<IValue> arguments, Scope s)
        {
            if (arguments.Count == 2)
            {
                MapTable map = (MapTable) arguments[0];
                return map[arguments[1]];
            }
            else
            {
                ListNode list = (ListNode) arguments[0];
                MapTable map = (MapTable) list.Children[1].Eval(s);
                IValue index = list.Children[2].EvalOnce(s);

                if (ReferenceEquals(index, Value.Nil))
                {
                    //Use the identifier
                    index = list.Children[2];
                }

                return map[index];
            }
        }

        private IValue SetKey(List<IValue> arguments, Scope s)
        {
            ListNode list = (ListNode)arguments[0];
            MapTable map = (MapTable)list.Children[1].Eval(s);
            IValue index = list.Children[2].Eval(s);
            IValue value = list.Children[3].Eval(s);

            //Reference comparison
            if (ReferenceEquals(index, Value.Nil))
            {
                //Use the identifier
                index = list.Children[2];
            }

            map.Assign(index, value);

            return map;
        }

        private IValue AreEqual(List<IValue> arguments, Scope s)
        {
            MapTable left = (MapTable)arguments[0];
            MapTable right = (MapTable)arguments[1];

            return left.AsDictionary().SequenceEqual(right.AsDictionary()) ? Bool.True : Bool.False;
        }

        private IValue Length(List<IValue> arguments, Scope s)
        {
            MapTable map = (MapTable)arguments.First();

            return map.Count;
        }
    }
}