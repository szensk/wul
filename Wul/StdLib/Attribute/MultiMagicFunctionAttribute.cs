using System;
using System.Runtime.CompilerServices;

namespace Wul.StdLib.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MultiMagicFunctionAttribute : System.Attribute, IWulFunction
    {
        public MultiMagicFunctionAttribute(
            string name,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0
        )
        {
            Name = name;
            FileName = file;
            Member = member;
            Line = line;
        }

        public string Name { get; }
        public string FileName { get; }
        public string Member { get; }
        public int Line { get; }
    }
}