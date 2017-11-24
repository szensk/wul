﻿using System.Collections.Generic;
using Wul.Parser;

namespace Wul.Interpreter
{
    public class MagicFunctionMetaType : MetaType
    {
        public MagicFunctionMetaType()
        {
            InvokeMagic.Method = new NetFunction(InvokeMagicFunction, InvokeMagic.Name);   
        }

        public IValue InvokeMagicFunction(List<IValue> arguments, Scope s)
        {
            IFunction function = (IFunction)arguments[0];
            ListNode listNode = (ListNode) arguments[1];
            return function.Execute(listNode, s);
        }

        //TODO AsString
    }
}