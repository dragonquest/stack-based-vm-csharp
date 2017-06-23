using System;
using System.Collections.Generic;

using VM;
using VM.Util;
using VM.Parsing;
using VM.Parsing.AST;

namespace VM.CodeGeneration.VMBytecode
{
    // Compiler compiles the tokens (AST) to
    // the VM language instructions
    public class Compiler : Visitor, IVisitor
    {
        private readonly List<int> _ins;
        private readonly global::VM.Bytecode _bytecode;
        private readonly SymbolTable _symbolTable;
        private readonly List<IError> _errors;

        public Compiler(SymbolTable symbolTable)
        {
            _ins = new List<int>();
            _bytecode = new global::VM.Bytecode();
            _symbolTable = symbolTable;

            _errors = new List<IError>();
        }

        public override void Visit(Instruction ins)
        {
            Tuple<int, global::VM.Bytecode.Instruction> opcodeWithInstruction = _bytecode.FindByName(ins.Name);
            var opcode = opcodeWithInstruction.Item1;
            var instruction = opcodeWithInstruction.Item2;

            if (opcode == -1)
            {
                _errors.Add(new Error(String.Format("Opcode for instruction '{0}' not found", instruction.Name)));
                return;
            }
            _ins.Add(opcode);

            if (ins.Parameters.Count > 0)
            {
                foreach (var param in ins.Parameters)
                {
                    if (param.Type == ParameterType.Integer)
                    {
                        _ins.Add(param.IntegerValue);
                    }
                    else if (param.Type == ParameterType.Label)
                    {
                        var label = _symbolTable.Lookup(SymbolType.Label, param.StringValue);
                        // Retrieving the address the label is pointing to:
                        if (label == null)
                        {
                           _errors.Add(new Error(String.Format("Label symbol could not be found: {0} declared on line {1}", param.StringValue, param.LineNumber)));
                           return;
                        }
                        else
                        {
                            _ins.Add(label.Address);
                        }
                    }
                }
            }
        }

        public List<int> GetOpcodes()
        {
            return _ins;
        }

        public List<IError> GetErrors()
        {
            return _errors;
        }
    }
}


