﻿using System;
using System.Runtime.CompilerServices;

namespace Wul.StdLib
{
    internal interface IWulFunction
    {
        string Name { get; }
        string FileName { get; }
        string Member { get; }
        int Line { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class NetFunctionAttribute : Attribute, IWulFunction
    {
        public NetFunctionAttribute(
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
    internal class MultiNetFunctionAttribute : Attribute, IWulFunction
    {
        public MultiNetFunctionAttribute(
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