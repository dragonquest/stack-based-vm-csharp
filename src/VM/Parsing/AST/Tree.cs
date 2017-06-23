using System.Collections.Generic;

namespace VM.Parsing.AST
{
    // Tree represents the "application"
    // which can contain n Line-Nodes.
    public class Tree : Node
    {
        public List<Line> Line;

        public Tree()
        {
            Line = new List<Line>();
        }
    }
}

