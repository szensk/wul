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

        private static bool RunFile(string filePath, Scope scope = null)
        {
            Parser = new ProgramParser(new FileInfo(filePath).FullName);
            try
            {
                WulInterpreter.Interpret(LoadFile(filePath), scope);
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
                //TODO fix up traceback line numbers
                //StdLib.Debug.Traceback(Value.EmptyList, null);
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
            Console.WriteLine("\t-i drop into interactive mode after evaluating file");
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

        private static int ReadEvalPrintLoop(string file = null, string runString = null)
        {
            Console.WriteLine($"wul interpreter {Version}");
            Console.WriteLine("to leave type 'exit'");

            Scope replScope = Global.Scope.EmptyChildScope();

            if (!string.IsNullOrWhiteSpace(file))
            {
                RunFile(file, replScope);
            }

            if (!string.IsNullOrWhiteSpace(runString))
            {
                RunString(runString, replScope);
            }

            while (true)
            {
                string input = Console.ReadLine();
                System.Diagnostics.Debug.WriteLine(input);
                if (input == "exit" || input == "q") break;
                RunString(input, replScope);
            }
            return ExitSuccess;
        }

        private class StartUpArguments
        {
            public static implicit operator StartUpArguments(int returnCode)
            {
                return new StartUpArguments
                {
                    ReturnCode = returnCode
                };
            }

            public int? ReturnCode { get; set; }
            public string FileName { get; set; }
            public string Script { get; set; }
            public bool REPL { get; set; }
        }

        private static StartUpArguments ParseArguments(string[] args)
        {
            bool dropToInterpreter = false;
            int evalArgument = -1;
            bool addParentheses = false;
            string fileName = null;
            string script = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg)
                {
                    case "-h":
                    case "--help":
                        return PrintHelp();
                    case "-v":
                    case "--version":
                        return PrintVersion();
                    case "-i":
                        dropToInterpreter = true;
                        break;
                    case "-ep":
                    case "-e":
                        if (i + 1 < args.Length)
                        {
                            evalArgument = ++i;
                        }
                        else
                        {
                            return Error("No script specified");
                        }
                        addParentheses = arg == "-ep";
                        break;
                    default:
                        if (arg.StartsWith('-'))
                        {
                            return Error($"Unrecognized option {arg}");
                        }
                        else
                        {
                            fileName = arg;
                        }
                        break;
                }
            }
            
            if (evalArgument >= 0)
            {
                script = args[evalArgument];
                script = addParentheses ? $"({script})" : script;
            }

            if (string.IsNullOrWhiteSpace(script) && string.IsNullOrWhiteSpace(fileName))
            {
                dropToInterpreter = true;
            }

            return new StartUpArguments
            {
                REPL = dropToInterpreter,
                FileName = fileName,
                Script = script
            };
        }

        private static int Main(string[] args)
        {
            Global.RegisterDefaultFunctions();

            var arguments = ParseArguments(args);
            if (arguments.ReturnCode.HasValue) return arguments.ReturnCode.Value;
            if (arguments.REPL)
            {
                return ReadEvalPrintLoop(arguments.FileName, arguments.Script);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(arguments.FileName))
                {
                    if (!RunFile(arguments.FileName))
                    {
                        return ExitError;
                    }
                }

                if (!string.IsNullOrWhiteSpace(arguments.Script))
                {
                    if (!RunString(arguments.Script))
                    {
                        return ExitError;
                    }
                }

                return ExitSuccess;
            }
        }
    }
}
