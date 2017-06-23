using System;
using System.Collections.Generic;

namespace VM
{
    public partial class Bytecode
    {
        public class Instruction
        {
            public readonly string Name;
            public readonly int NumArgs;

            public Instruction(string name, int numArgs)
            {
                Name = name;
                NumArgs = numArgs;
            }
        }

        // FindByName returns -1 and null if the instruction
        // could not be found.
        public Tuple<int, Instruction> FindByName(string name)
        {
            foreach (var ins in _insTable)
            {
                if (String.Equals(name, ins.Value.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return new Tuple<int, Instruction>(ins.Key, ins.Value);
                }
            }
            return new Tuple<int, Instruction>(-1, null);
        }
    }
}

