using System;
using System.Collections.Generic;

using VM;
using VM.Util;
using VM.Parsing.AST;

namespace VM.Parsing
{
    // The SemanticAnalyzer checks whether the constructed parse tree
    // follows the rules of the language.
    // Currently it checks if the instructions do exist and if they have
    // the required parameters.
    public class SemanticAnalyzer : Visitor, IVisitor
    {
        private Bytecode _bytecode;
        private List<IError> _errors;

        public SemanticAnalyzer()
        {
            _bytecode = new global::VM.Bytecode();
            _errors = new List<IError>();
        }

        public override void Visit(Instruction ins)
        {
            Tuple<int, global::VM.Bytecode.Instruction> opcodeWithInstruction = _bytecode.FindByName(ins.Name);
            var opcode = opcodeWithInstruction.Item1;
            var instruction = opcodeWithInstruction.Item2;

            if (opcode == -1)
            {
                _errors.Add(new Error(String.Format("Error on line {0}: Opcode for instruction '{1}' not found.", ins.LineNumber, ins.Name)));
                return;
            }

            if (instruction.NumArgs != ins.Parameters.Count)
            {
                _errors.Add(new Error(String.Format("Error on line {0}: Instruction '{1}' has wrong parameter count. Expected {2}, got {3}", ins.LineNumber, instruction.Name, instruction.NumArgs, ins.Parameters.Count)));
                return;
            }
        }

        public List<IError> GetErrors()
        {
            return _errors;
        }
    }
}


