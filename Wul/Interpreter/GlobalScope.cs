namespace Wul.Interpreter
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

            //IO
            Scope["print"] = StdLib.IO.Print;
            Scope["clear"] = StdLib.IO.Clear;

            //Arith
            Scope["+"] = StdLib.Arithmetic.Add;
            Scope["-"] = StdLib.Arithmetic.Subtract;
            Scope["<"] = StdLib.Arithmetic.LessThan;
        }
    }
}
