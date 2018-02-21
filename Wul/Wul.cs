using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;
using Wul.StdLib;

namespace Wul
{
    //WUL: Worthless Unnecessary Language
    class Wul
    {
        private const int ExitSuccess = 0;
        private const int ExitError = 1;

        private static readonly ProgramParser Parser = new ProgramParser();

        private static string Version => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        
        private static ProgramNode LoadFile(string filePath)
        {
            string programText = File.ReadAllText(filePath);
            return (ProgramNode) Parser.Parse(programText);
        }

        private static bool RunFile(string filePath)
        {
            try
            {
                WulInterpreter.Interpret(LoadFile(filePath));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
#if DEBUG
                Console.WriteLine(e.StackTrace);
#endif
                return false;
            }
            return true;
        }

        private static bool RunString(string input, Scope scope = null)
        {
            Scope currentScope = scope ?? Global.Scope.EmptyChildScope();

            try
            {
                var programNode = (ProgramNode) Parser.Parse(input.Trim());
                var result = WulInterpreter.Interpret(programNode, currentScope);
                if (result != null && result.Any())
                {
                    foreach (var item in result)
                    {
                        if (ReferenceEquals(item, Value.Nil) && result.Count == 1) continue;
                        var args = new List<IValue> {item is UString ? new UString($"'{item.AsString()}'") : item};
                        IO.Print(args, Global.Scope);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("wul [file]");
            Console.WriteLine($"worthless unnecessary language version {Version}");
            Console.WriteLine("\t-e program to evaluate");
            Console.WriteLine("\t-h help");
        }

        private static int Error(string message)
        {
            Console.WriteLine(message);
            PrintHelp();
            return ExitError;
        }

        private static int Main(string[] args)
        {
            Global.RegisterDefaultFunctions();

            if (!args.Any())
            {
                string input = "";

                Scope replScope = Global.Scope.EmptyChildScope();

                Console.WriteLine($"wul interpreter {Version}");
                Console.WriteLine("to leave type 'exit'");

                while (input != "exit")
                {
                    input = Console.ReadLine();
                    RunString(input, replScope);
                }
                return ExitSuccess;
            }

            if (args.Length == 1)
            {
                if (args[0] == "-h")
                {
                    PrintHelp();
                    return ExitSuccess;
                }
                if (args[0].StartsWith('-'))
                {
                    return Error($"Unrecognized option {args[0]}");
                }

                string filePath = args[0];
                if (File.Exists(filePath))
                {
                    return RunFile(filePath) ? ExitSuccess : ExitError;
                }
                return Error($"Unable to open file {filePath}");
            }
            if (args.Length == 2)
            {
                if (args[0] != "-e" && args[0] != "-ep")
                {
                    return Error($"Unrecognized or invalid option {args[0]}");
                }
                string input = args[0] == "-ep" ? $"({args[1]})" : args[1];
                return RunString(input) ? ExitSuccess : ExitError;
            }

            PrintHelp();
            return ExitSuccess;
        }
    }
}
