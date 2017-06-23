using System;
using System.Text;

namespace VM.Parsing.AST
{
    public partial class PrettyPrinter : Visitor, IVisitor
    {
        // StringBuilderDecorater does some accounting
        // of characters written in order to align the
        // comments (after an instruction) properly.
        private class StringBuilderDecorator
        {
            private readonly StringBuilder _str;
            private int _pos;

            public StringBuilderDecorator(StringBuilder strBuilder)
            {
                _str = strBuilder;
                _pos = 0;
            }

            public void ResetPos()
            {
                _pos = 0;
            }

            public void Append(string text)
            {
                _str.Append(text);

                if (text != "\n")
                {
                    _pos += text.Length;
                }
            }

            public void AppendWithPadding(string text)
            {
                int padCount = 25 - _pos;
                if (padCount > 0)
                {
                    _str.Append(new String(' ', padCount));
                }

                _str.Append(text);
                _pos += text.Length;
            }

            public StringBuilder GetStringBuilder()
            {
                return _str;
            }
        }

    }
}
