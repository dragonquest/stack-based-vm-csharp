using System.Collections.Generic;

namespace VM.Parsing.AST
{
    // Walker traverses the AST and passes each
    // Node element to the visitor.
    // Current limitation is that it only accepts one
    // visitor.
    public class Walker
    {
        public void Walk(IVisitor visitor, Node node)
        {
            if (node is Tree)
            {
                walkTree(visitor, node as Tree);
            }
            else if (node is Instruction)
            {
                walkInstruction(visitor, node as Instruction);
            }
        }

        private void walkTree(IVisitor visitor, Tree tree)
        {
           foreach (var line in tree.Line)
           {
                visitor.VisitEnter(line);

                switch (line.Type)
                {
                    case LineType.Instruction:
                        walkInstruction(visitor, line.Instruction);
                        break;
                }

                visitor.VisitLeave(line);
           }

        }

        private void walkInstruction(IVisitor visitor, Instruction ins)
        {
            visitor.Visit(ins);
            if (ins.Parameters.Count > 0)
            {
                foreach (var param in ins.Parameters)
                {
                    visitor.Visit(param);
                }
            }
        }
    }
}
