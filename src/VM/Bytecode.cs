using System;
using System.Collections.Generic;

namespace VM
{
    public partial class Bytecode
    {
        public const int Push = 1;
        public const int IAdd = 2;
        public const int IMult = 3;
        public const int GStore = 4;
        public const int GLoad = 5;
        public const int Jump = 6;
        public const int JumpTrue = 7;
        public const int JumpFalse = 8;
        public const int IEqual = 9;
        public const int ILessThan = 10;
        public const int ISub = 11;
        public const int IDiv = 12;
        public const int Print = 13;
        public const int PrintChar = 14;
        public const int Halt = 15;
        public const int Call = 16;
        public const int Ret = 17;
        public const int Load = 18;

        private readonly Dictionary<int, Instruction> _insTable;

        public Bytecode()
        {
            _insTable = new Dictionary<int, Instruction>()
            {
                {Push, new Instruction("PUSH", 1)},
                {IAdd, new Instruction("IADD", 0)},
                {ISub, new Instruction("ISUB", 0)},
                {IMult, new Instruction("IMULT", 0)},
                {IDiv, new Instruction("IDIV", 0)},
                {IEqual, new Instruction("IEQUAL", 0)},
                {ILessThan, new Instruction("ILESSTHAN", 0)},
                {GStore, new Instruction("GSTORE", 1)},
                {GLoad, new Instruction("GLOAD", 1)},
                {Jump, new Instruction("JUMP", 1)},
                {JumpTrue, new Instruction("JUMPTRUE", 1)},
                {JumpFalse, new Instruction("JUMPFALSE", 1)},
                {Print, new Instruction("PRINT", 0)},
                {PrintChar, new Instruction("PRINTCHAR", 0)},
                {Halt, new Instruction("HALT", 0)},
                {Call, new Instruction("CALL", 2)},
                {Ret, new Instruction("RET", 0)},
                {Load, new Instruction("LOAD", 1)},
            };
        }

        // Returns the name of the instruction opcode or
        // returns an empty string when not found
        public string FindNameByOpcode(int opcode)
        {
            if (!_insTable.ContainsKey(opcode))
            {
                return "";
            }

            return _insTable[opcode].Name;
        }

        // Returns the number of arguments a opcode has or
        // -1 if the instruction opcode could not be found.
        public int GetNumArgs(int opcode)
        {
            if (!_insTable.ContainsKey(opcode))
            {
                return -1;
            }

            return _insTable[opcode].NumArgs;
        }
    }

}
