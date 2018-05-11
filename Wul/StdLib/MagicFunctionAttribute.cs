using System;
using System.Runtime.CompilerServices;

namespace Wul.StdLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MagicFunctionAttribute : Attribute, IWulFunction 
    {
        public MagicFunctionAttribute(
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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class MultiMagicFunctionAttribute : Attribute, IWulFunction
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
