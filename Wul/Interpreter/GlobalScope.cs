﻿namespace Wul.Interpreter
{
    class Global
    {
        private static Scope _scope;

        public static Scope Scope => _scope ?? (_scope = new Scope(null));

        public static void RegisterDefaultFunctions()
        {
            //General
            Scope["nil"] = Value.Nil;
            Scope["let"] = StdLib.General.Let;
            Scope["def"] = StdLib.General.Define;
            Scope["@def"] = StdLib.General.DefineMagicFunction;
            Scope["eval"] = StdLib.General.Evaluate;
            Scope["lambda"] = StdLib.General.Lambda;

            //Comparison
            Scope["="] = StdLib.Comparison.Equal;
            Scope["<"] = StdLib.Comparison.LessThan;

            //Conditional
            Scope["if"] = StdLib.General.If;
            Scope["then"] = StdLib.General.Then;
            Scope["else"] = StdLib.General.Then; //Not a typo!

            //Bools
            Scope["true"] = Bool.True;
            Scope["false"] = Bool.False;

            //IO
            Scope["print"] = StdLib.IO.Print;
            Scope["clear"] = StdLib.IO.Clear;

            //Arith
            Scope["+"] = StdLib.Arithmetic.Add;
            Scope["-"] = StdLib.Arithmetic.Subtract;
            Scope["*"] = StdLib.Arithmetic.Multiply;

            //String
            Scope[".."] = StdLib.String.Concat;
            Scope["substring"] = StdLib.String.Substring;
            Scope["lower"] = StdLib.String.Lower;
            Scope["upper"] = StdLib.String.Upper;

            //List
            Scope["concat"] = StdLib.List.Concat;
            Scope["first"] = StdLib.List.First;
            Scope["rem"] = StdLib.List.Remainder;
            Scope["empty?"] = StdLib.List.Empty;
            Scope["len"] = StdLib.List.Length;
            Scope["#"] = StdLib.List.Length;

            //Logical
            Scope["!"] = StdLib.Logical.Not;
            Scope["not"] = StdLib.Logical.Not;
        }
    }
}