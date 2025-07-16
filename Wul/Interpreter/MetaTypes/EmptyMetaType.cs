using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wul.Interpreter.MetaTypes
{
    internal class EmptyMetaType : MetaType
    {
        public static readonly EmptyMetaType Instance = new();

    }
}
