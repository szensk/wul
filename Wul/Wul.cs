using System;
using System.Collections.Generic;
using Wul.Interpreter;
using Wul.Parser;

namespace Wul
{
    //WUL
    //Worthless unnecessary language
    class Wul
    {
        static void Main(string[] args)
        {
            //built ins: if, then, else
            var parser = new ProgramParser();
            Global.RegisterDefaultFunctions();

            string input = "";
            while (input != "exit")
            {
                input = Console.ReadLine();
                var programNode = (ProgramNode) parser.Parse(input.Trim());
                var result = WulInterpreter.Interpret(programNode);
                if (result != null && result != Value.Nil)
                {
                    StdLib.IO.Print.Evaluate(new List<IValue>{result}, Global.Scope);
                }
            }
        }
    }
}
