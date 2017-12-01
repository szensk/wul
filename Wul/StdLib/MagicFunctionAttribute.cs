using System;

namespace Wul.StdLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MagicFunctionAttribute : Attribute
    {
        public string Name { get; set; }

        public MagicFunctionAttribute(string name)
        {
            Name = name;
        }
    }
}
