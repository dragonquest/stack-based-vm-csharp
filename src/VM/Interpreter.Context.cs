using System.Collections.Generic;

namespace VM
{
    public partial class Interpreter
    {
        public class Context
        {
            public readonly List<int> Code;
            public int IP = 0; // Instruction Pointer

            public List<int> Stack;
            public int SP = -1; // Stack pointer
            public int BP = -1; // Base stack pointer

            public int[] Globals; // Global data

            public Context(List<int> code)
            {
                Code = code;

                Stack = new List<int>();
                Globals = new int[250];
            }
        }
    }
}
