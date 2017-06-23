using System;
using System.Collections.Generic;

using VM;
using VM.Parsing;
using VM.Parsing.AST;

namespace VM.CodeGeneration.VMBytecode
{
    // The SymbolCollector collects all the symbols
    // so they can be referenced to & used during the
    // compilation phase
    public class SymbolCollector : Visitor, IVisitor
    {
        private SymbolTable _symbolTable;
        private int _address;

        public SymbolCollector()
        {
            _symbolTable = new SymbolTable();
            _address = 0;
        }

        public override void VisitEnter(Line line)
        {
            if (line.Type == LineType.Label)
            {
                _symbolTable.Add(new Symbol(line.Label, SymbolType.Label, _address));
            }
        }

        public override void Visit(Instruction ins)
        {
            _address += (1 + ins.Parameters.Count);
        }

        public SymbolTable GetSymbolTable()
        {
            return _symbolTable;
        }
    }
}


