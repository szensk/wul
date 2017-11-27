using System;

namespace Wul.StdLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class GlobalNameAttribute : Attribute
    {
        public string Name { get; set; }

        public GlobalNameAttribute(string name)
        {
            Name = name;
        }
    }
}
