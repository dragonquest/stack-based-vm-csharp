using System.Collections.Generic;

namespace VM.Parsing.AST
{
    public class Visitor 
    {
        public virtual void VisitEnter(Line line) {}
        public virtual void VisitLeave(Line line) {}
        public virtual void Visit(Instruction ins) {}
        public virtual void Visit(Parameter param) {}
    }
}
