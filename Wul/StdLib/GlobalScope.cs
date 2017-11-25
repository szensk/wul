using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    class Global
    {
        private static Scope _scope;

        public static Scope Scope => _scope ?? (_scope = new Scope(null));

        public static void RegisterDefaultFunctions()
        {
            //General
            Scope["nil"] = Value.Nil; //unnecessary as anything undefined will return nil as well
            Scope["let"] = General.Let;
            Scope["def"] = General.Define;
            Scope["@def"] = General.DefineMagicFunction;
            Scope["eval"] = General.Evaluate;
            Scope["lambda"] = General.Lambda;
            Scope["identity"] = General.Identity;
            Scope["??"] = General.Coalesce;

            //MetaMethod
            Scope["set-metamethod"] = MetaType.SetMetaType;

            //Interop
            Scope["::"] = Interop.CallFrameworkFunction;

            //Comparison
            Scope["="] = Comparison.Equal;
            Scope["<"] = Comparison.LessThan;

            //Conditional
            Scope["if"] = General.If;
            Scope["then"] = General.Then;
            Scope["else"] = General.Then; //Not a typo!

            //Bools
            Scope["true"] = Bool.True;
            Scope["false"] = Bool.False;

            //IO
            Scope["print"] = IO.Print;
            Scope["clear"] = IO.Clear;

            //Arith
            Scope["+"] = Arithmetic.Add;
            Scope["-"] = Arithmetic.Subtract;
            Scope["*"] = Arithmetic.Multiply;

            //String
            Scope[".."] = String.Concat;
            Scope["substring"] = String.Substring;
            Scope["lower"] = String.Lower;
            Scope["upper"] = String.Upper;
            Scope["string"] = String.Stringify;

            //List
            Scope["concat"] = String.Concat;
            Scope["first"] = List.First;
            Scope["last"] = List.Last;
            Scope["rem"] = List.Remainder;
            Scope["empty?"] = List.Empty;
            Scope["len"] = List.Length;
            Scope["#"] = List.Length;
            Scope["list"] = List.Listify;

            //Logical
            Scope["!"] = Logical.Not;
            Scope["not"] = Logical.Not;
        }
    }
}
