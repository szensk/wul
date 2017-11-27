using System;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    internal class Map
    {
        internal static IFunction Dictionary = new NetFunction((list, scope) =>
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
        }, "dict");

        internal static IFunction Object = new MagicNetFunction((list, scope) =>
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
        }, "object");

    }
}
