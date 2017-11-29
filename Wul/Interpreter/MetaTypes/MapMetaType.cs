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
            Equal.Method = new NetFunction(AreEqual, Equal.Name);

            //List
            At.Method = new NetFunction(AtKey, At.Name);
            Set.Method = new NetFunction(SetKey, Set.Name);
            Count.Method = new NetFunction(Length, Count.Name);

            //Other
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
        }

        public IValue AtKey(List<IValue> arguments, Scope s)
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

                //Reference comparison
                if (index == Value.Nil)
                {
                    //Use the identifier
                    index = list.Children[2];
                }

                return map[index];
            }
        }

        public IValue SetKey(List<IValue> arguments, Scope s)
        {
            ListNode list = (ListNode)arguments[0];
            MapTable map = (MapTable)list.Children[1].Eval(s);
            IValue index = list.Children[2].Eval(s);
            IValue value = list.Children[3].Eval(s);

            //Reference comparison
            if (index == Value.Nil)
            {
                //Use the identifier
                index = list.Children[2];
            }

            map.Assign(index, value);

            return map;
        }

        public IValue AreEqual(List<IValue> arguments, Scope s)
        {
            MapTable left = (MapTable)arguments[0];
            MapTable right = (MapTable)arguments[1];

            return left.AsDictionary().SequenceEqual(right.AsDictionary()) ? Bool.True : Bool.False;
        }

        public IValue Length(List<IValue> arguments, Scope s)
        {
            MapTable map = (MapTable)arguments.First();

            return map.Count;
        }
    }
}