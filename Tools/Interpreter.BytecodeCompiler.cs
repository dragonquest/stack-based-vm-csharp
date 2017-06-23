using System;
using System.IO;
using System.Collections.Generic;

using VM;
using VM.Parsing;
using VM.Parsing.AST;

public partial class Interpreter
{
    private class BytecodeCompiler
    {
        public BytecodeCompiler() {}

        public List<int> FileToInstructions(string inputFile)
        {
            string source = File.ReadAllText(inputFile);

            VM.Parsing.AST.Tree tree;
            var errors = VM.Parser.TryParse(source, out tree);
            if (errors.Count > 0)
            {
                foreach (var err in errors)
                {
                    Console.Error.WriteLine(err);
                }
                return new List<int>();
            }

            var treeWalker = new Walker();

            var symbolCollector = new VM.CodeGeneration.VMBytecode.SymbolCollector();
            treeWalker.Walk(symbolCollector, tree);

            var symbolTable = symbolCollector.GetSymbolTable();

            var compiler = new VM.CodeGeneration.VMBytecode.Compiler(symbolTable);
            treeWalker.Walk(compiler, tree);
            if (compiler.GetErrors().Count > 0)
            {
                foreach (var err in compiler.GetErrors())
                {
                    Console.Error.WriteLine(err);
                }
                return new List<int>();
            }

            return compiler.GetOpcodes();
        }
    }
}
