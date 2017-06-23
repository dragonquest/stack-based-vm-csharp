namespace VM.Parsing.AST
{
    public enum LineType
    {
        Instruction,
        Label,
        Comment
    }

    public class Line : Node
    {
        public LineType Type;

        public Instruction Instruction;
        public string Label;
        public string Comment;

        public Line(LineType type, object obj)
        {
            Type = type;
            Comment = "";

            if (type == LineType.Comment)
            {
                Comment = (string)obj;
            }
            else if (type == LineType.Label)
            {
                Label = (string)obj;
            }
            else if (type == LineType.Instruction)
            {
                Instruction = (Instruction)obj;
            }
        }
    }
}

