using System.Collections.Generic;

namespace VM.Parsing.AST
{
    public interface IVisitor
    {
        void VisitEnter(Line line);
        void VisitLeave(Line line);
        void Visit(Instruction ins);
        void Visit(Parameter param);
    }
}
