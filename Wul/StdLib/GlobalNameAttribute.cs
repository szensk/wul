using System;

namespace Wul.StdLib
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true)]
    internal class GlobalNameAttribute : Attribute
    {
        public string Name { get; set; }

        public GlobalNameAttribute(string name)
        {
            Name = name;
        }
    }
}
