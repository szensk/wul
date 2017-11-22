using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Parser;

namespace Wul
{
    //WUL: Worthless Unnecessary Language
    class Wul
    {
        private static readonly ProgramParser parser = new ProgramParser();

        private static string Version => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        
        private static ProgramNode LoadFile(string filePath)
        {
            string programText = File.ReadAllText(filePath);
            return (ProgramNode) parser.Parse(programText);
        }

        private static bool RunFile(string filePath)
        {
            var result = WulInterpreter.Interpret(LoadFile(filePath));
            return true; //TODO 
        }

        private static void PrintHelp()
        {
            Console.WriteLine("wul - worthless unnecessary language");
            Console.WriteLine($"version {Version}");
            Console.WriteLine("\t-e filepath");
            Console.WriteLine("\t-h help");
        }

        private static int Error(string message)
        {
            Console.WriteLine(message);
            PrintHelp();
            return -1;
        }

        private static int Main(string[] args)
        {
            if (!args.Any())
            {
                Global.RegisterDefaultFunctions();

                string input = "";

                Console.WriteLine($"wul interpreter {Version}");
                Console.WriteLine("to leave type 'exit'");

                while (input != "exit")
                {
                    input = Console.ReadLine();
                    var programNode = (ProgramNode) parser.Parse(input.Trim());
                    var result = WulInterpreter.Interpret(programNode);
                    if (result != null && result != Value.Nil)
                    {
                        StdLib.IO.Print.Evaluate(new List<IValue> {result}, Global.Scope);
                    }
                }
                return 0;
            }
            else if (args.Length == 1)
            {
                if (args[0] == "-h")
                {
                    PrintHelp();
                    return 0;
                }
                else
                {
                    return Error($"Unrecognized or invalid option {args[0]}");
                }
            }
            else if (args.Length == 2)
            {
                if (args[0] == "-e")
                {
                    string filePath = args[1];
                    if (File.Exists(filePath))
                    {
                        RunFile(filePath);
                    }
                    else
                    {
                        Console.WriteLine($"Unable to open file {filePath}");
                        return -1;
                    }
                }
                else
                {
                    return Error($"Unrecognized or invalid option {args[0]}");
                }
            }
            else
            {
                PrintHelp();
                return 0;
            }
            return 0;
        }
    }
}
