using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using VM;
using VM.Parsing;
using VM.Parsing.AST;

namespace VM.CodeGeneration.MSIL
{
    // The SymbolCollector collects all the symbols
    // so they can be referenced to & used during the
    // compilation phase
    public class SymbolCollector : Visitor, IVisitor
    {
        private readonly SymbolTable _symbolTable;
        private readonly ILGenerator _gen;

        public SymbolCollector(ILGenerator gen)
        {
            _gen = gen;
            _symbolTable = new SymbolTable();
        }

        public override void VisitEnter(Line line)
        {
            if (line.Type == LineType.Label)
            {
                var pseudoAddress = 0; // just a filler

                _symbolTable.Add(new MSILSymbol(line.Label, SymbolType.Label, pseudoAddress, _gen.DefineLabel()));
            }
        }

        public SymbolTable GetSymbolTable()
        {
            return _symbolTable;
        }
    }
}


