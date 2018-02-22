using System;
using System.IO;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    internal class Module
    {
        [MagicFunction("import")]
        internal static IValue Import(ListNode list, Scope scope)
        {
            IdentifierNode fileNameIdentifier;
            IdentifierNode importIdentifier;
            if (list.Children.Count == 2)
            {
                fileNameIdentifier = importIdentifier = (IdentifierNode) list.Children[1];
                
            }
            else if (list.Children.Count == 4)
            {
                fileNameIdentifier = (IdentifierNode) list.Children[1];
                importIdentifier = (IdentifierNode) list.Children[3];
            }
            else
            {
                throw new Exception("wrong number of arguments for import");
            }

            string fileName = fileNameIdentifier.Name.Replace('.', Path.DirectorySeparatorChar) + ".wul";
            string targetName = importIdentifier.Name;

            Scope importScope = Global.Scope.EmptyChildScope();

            string cwd = Directory.GetCurrentDirectory();
            string fullFileName = Path.Combine(cwd, fileName);
            var fileInfo = new FileInfo(fullFileName);
            if (fileInfo.Exists)
            {
                ProgramParser Parser = new ProgramParser(fullFileName);
                string programText = File.ReadAllText(fileInfo.FullName);
                ProgramNode programNode = (ProgramNode) Parser.Parse(programText);
                var result = WulInterpreter.Interpret(programNode, importScope);

                foreach (var binding in importScope.BoundVariables)
                {
                    // Only upper case definitions are exported
                    if (char.IsUpper(binding.Key[0])) scope[targetName + "." + binding.Key] = binding.Value.Value;
                }

                return result.First();
            }
            else
            {
                throw new Exception($"file {fileInfo.FullName} could not be found");
            }
        }
    }
}
