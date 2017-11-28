using System;

namespace Wul.StdLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class NetFunctionAttribute : Attribute
    {
        public string Name { get; set; }

        public NetFunctionAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MagicNetFunctionAttribute : Attribute
    {
        public string Name { get; set; }

        public MagicNetFunctionAttribute(string name)
        {
            Name = name;
        }
    }
}
