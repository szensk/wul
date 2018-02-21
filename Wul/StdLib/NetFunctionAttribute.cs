using System;

namespace Wul.StdLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class NetFunctionAttribute : Attribute
    {
        public string Name { get; }

        public NetFunctionAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MultiNetFunctionAttribute : NetFunctionAttribute
    {
        public MultiNetFunctionAttribute(string name) : base(name)
        {
        }
    }
}