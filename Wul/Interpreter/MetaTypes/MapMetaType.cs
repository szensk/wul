﻿using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class MapMetaType : MetaType
    {
        public static readonly MapMetaType Instance = new MapMetaType();

        //TODO 
        private MapMetaType()
        {
            //AsString
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
        }
    }
}