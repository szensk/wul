using System;

namespace Wul.StdLib
{
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
