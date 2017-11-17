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
            //built ins: def, if, -, <, then, else
            var interpreter = new Interpreter.WulInterpreter();
            var parser = new ProgramParser();

            string input = "";
            while (input != "exit")
            {
                input = Console.ReadLine();
                var programNode = (ProgramNode) parser.Parse(input.Trim());
                var result = interpreter.Interpret(programNode);
                if (result != null && result != Value.Nil)
                {
                    StdLib.IO.Print.Evaluate(new List<IValue>{result});
                }
            }
        }
    }
}
