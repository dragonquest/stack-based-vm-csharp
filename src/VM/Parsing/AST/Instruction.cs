using System.Collections.Generic;

namespace VM.Parsing.AST
{
    public class Instruction : Node
    {
        public string Name;
        public List<Parameter> Parameters;

        public Instruction(string name)
        {
            Name = name;
            Parameters = new List<Parameter>();
        }
    }
}
