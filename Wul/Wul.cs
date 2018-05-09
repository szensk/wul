using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;
using Wul.Parser.Nodes;
using Wul.Parser.Parsers;
using Wul.StdLib;

namespace Wul
{
    //WUL: Worthless Unnecessary Language
    static class Wul
    {
        private const int ExitSuccess = 0;
        private const int ExitError = 1;

        private static ProgramParser Parser;

        private static string Version => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        
        private static ProgramNode LoadFile(string filePath)
        {
            string programText = File.ReadAllText(filePath);
            return (ProgramNode) Parser.Parse(programText);
        }

        private static bool RunFile(string filePath)
        {
            Parser = new ProgramParser(new FileInfo(filePath).FullName);
            try
            {
                WulInterpreter.Interpret(LoadFile(filePath));
            }
            catch (ParseException pe)
            {
                string program = File.ReadLines(filePath).Skip(pe.Line-1).First();
                Console.WriteLine(program);
                string underline = pe.GetUnderline;
                if (underline != null) Console.WriteLine(underline);
                Console.WriteLine(pe.GetErrorMessage);
                return false;
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
            Parser = new ProgramParser("stdin");
            Scope currentScope = scope ?? Global.Scope.EmptyChildScope();

            try
            {
                var programNode = (ProgramNode) Parser.Parse(input);
                var result = WulInterpreter.Interpret(programNode, currentScope);
                if (result != null && result.Any())
                {
                    foreach (var item in result)
                    {
                        if (ReferenceEquals(item, Value.Nil) && result.Count == 1) continue;
                        var args = new List<IValue> {item is WulString ? new WulString($"'{item.AsString()}'") : item};
                        IO.Print(args, Global.Scope);
                    }
                }
                return true;
            }
            catch (ParseException pe)
            {
                string underline = pe.GetUnderline;
                if (underline != null) Console.WriteLine(underline);
                Console.WriteLine(pe.GetErrorMessage);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        private static int PrintVersion()
        {
            Console.WriteLine($"v{Version}");
            return ExitSuccess;
        }

        private static int PrintHelp()
        {
            Console.WriteLine("usage: wul [file]");
            Console.WriteLine("\t-e program to evaluate");
            Console.WriteLine("\t-ep program to evaluate without outermost parenthesis");
            Console.WriteLine("\t-h help");
            Console.WriteLine("\t-v version");
            return ExitSuccess;
        }

        private static int Error(string message)
        {
            Console.WriteLine(message);
            PrintHelp();
            return ExitError;
        }

        private static int ReadEvalPrintLoop()
        {
            string input = "";

            Scope replScope = Global.Scope.EmptyChildScope();

            Console.WriteLine($"wul interpreter {Version}");
            Console.WriteLine("to leave type 'exit'");

            while (true)
            {
                input = Console.ReadLine();
                System.Diagnostics.Debug.WriteLine(input);
                if (input == "exit" || input == "q") break;
                RunString(input, replScope);
            }
            return ExitSuccess;
        }

        private static int Main(string[] args)
        {
            Global.RegisterDefaultFunctions();

            switch (args.Length)
            {
                case 0:
                    return ReadEvalPrintLoop();
                case 1:
                    switch (args[0])
                    {
                        case "-h":
                            return PrintHelp();
                        case "-v":
                            return PrintVersion();
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
                case 2:
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
