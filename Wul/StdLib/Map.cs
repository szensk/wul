using System;
using System.Collections.Generic;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    internal class Map
    {
        [NetFunction("dict")]
        internal static IValue Dictionary(List<IValue> list, Scope scope)
        {
            var mapList = ((ListTable) list[0]).AsList();

            if (mapList.Count % 2 != 0)
            {
                throw new Exception("Unable to create map, missing key");
            }

            MapTable map = new MapTable();
            for(int i = 0; i < mapList.Count - 1; i += 2)
            {
                var key = mapList[i];
                var val = mapList[i + 1];
                map.Add(key, val);
            }

            return map;
        }

        [MagicNetFunction("object")]
        internal static IValue Object(ListNode list, Scope scope)
        {
            var mapList = ((ListNode) list.Children[1]).Children;

            if (mapList.Count % 2 != 0)
            {
                throw new Exception("Unable to create map, missing key");
            }

            MapTable map = new MapTable();
            for (int i = 0; i < mapList.Count - 1; i += 2)
            {
                var key = (IdentifierNode) mapList[i];
                var val = mapList[i + 1].Eval(scope);
                map.Add(key, val);
            }

            return map;
        }

        [MagicNetFunction("@object")]
        internal static IValue ObjectShort(ListNode list, Scope scope)
        {
            var mapList = ((ListNode)list.Children[1]).Children;

            MapTable map = new MapTable();
            for (int i = 0; i < mapList.Count; ++i)
            {
                var key = (IdentifierNode)mapList[i];
                var val = key.Eval(scope);
                map.Add(key, val);
            }

            return map;
        }

    }
}
