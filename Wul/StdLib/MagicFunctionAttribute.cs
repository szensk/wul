using System;

namespace Wul.StdLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MagicFunctionAttribute : Attribute
    {
        public string Name { get; }

        public MagicFunctionAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MultipleMagicFunctionAttribute : MagicFunctionAttribute
    {
        public MultipleMagicFunctionAttribute(string name) : base(name)
        {
        }
    }
}
