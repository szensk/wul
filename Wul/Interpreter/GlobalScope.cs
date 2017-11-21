namespace Wul.Interpreter
{
    class Global
    {
        private static Scope _scope;

        public static Scope Scope => _scope ?? (_scope = new Scope(null));

        public static void RegisterDefaultFunctions()
        {
            if (Scope.Parent == null)
            {
                //General
                Scope["nil"] = Value.Nil;
                Scope["let"] = StdLib.General.Let;
                Scope["def"] = StdLib.General.Define;
                Scope["def!"] = StdLib.General.DefineMagicFunction;
                Scope["eval"] = StdLib.General.Evaluate;

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
            }
        }
    }
}
