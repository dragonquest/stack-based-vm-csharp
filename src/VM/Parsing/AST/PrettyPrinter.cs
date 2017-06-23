using System;
using System.Text;
using VM.Parsing.AST;

namespace VM.Parsing.AST
{
    // PrettyPrinter Visitor walks through the AST and
    // formats the code.
    public partial class PrettyPrinter : Visitor, IVisitor
    {
        private readonly StringBuilderDecorator _str;
        private int _lastLine;

        public PrettyPrinter()
        {
            _str = new StringBuilderDecorator(new StringBuilder());
            _lastLine = 0;
        }

        public override void VisitEnter(Line line)
        {
            _str.ResetPos();

            maybeAddBlankLines(line);
            if (line.Type == LineType.Comment)
            {
                _str.Append("#" + line.Comment);
            }
            else if (line.Type == LineType.Label)
            {
                _str.Append(line.Label + ":");
            }
        }

        public override void VisitLeave(Line line)
        {
            // Considering comments after a line:
            if (hasCommentOnSameLine(line))
            {
                _str.AppendWithPadding("#" + line.Comment);
            }
            _str.Append("\n");
        }

        public override void Visit(Instruction ins)
        {
            _str.Append("  " + ins.Name.ToUpper());
            if (ins.Parameters.Count > 0)
            {
                foreach (var param in ins.Parameters)
                {
                    if (param.Type == ParameterType.Integer)
                    {
                        _str.Append(" " + param.IntegerValue);
                    }
                    else if (param.Type == ParameterType.Label)
                    {
                        _str.Append(" @" + param.StringValue);
                    }
                }
            }
        }

        // GetOutput returns the formatted code as a string.
        public string GetOutput()
        {
            return _str.GetStringBuilder().ToString();
        }

        private void maybeAddBlankLines(Line line)
        {
            if (_lastLine+1 < line.LineNumber)
            {
                for (int i = _lastLine+1; i < line.LineNumber; i++)
                {
                    _str.Append("\n");
                }
            }
            _lastLine = line.LineNumber;
        }

        private bool hasCommentOnSameLine(Line line)
        {
            return line.Type != LineType.Comment && line.Comment != "";
        }
    }
}

