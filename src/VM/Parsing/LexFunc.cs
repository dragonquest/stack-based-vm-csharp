using System;

namespace VM.Parsing
{
    internal class LexFunc
    {
        // Start checks if the input is either a
        // comment or an instruction:
        internal static StateFunc Start(Lexer lexer)
        {
            lexer.AcceptAnyRun(" \t\n");
            lexer.Ignore();

            if (lexer.Peek() == Lexer.EOF)
            {
                return null;
            }
            else if (lexer.Peek() == '#')
            {
                lexer.Next();
                lexer.Ignore();
                return Comment;
            }
            else if (!Char.IsLetter(lexer.Peek()))
            {
                return lexer.Error("Invalid input '" + lexer.Peek() + "' at pos: " + lexer.GetPos());
            }

            return Instruction;
        }

        // Checks whether the next string part is an instruction
        // or a label
        internal static StateFunc Instruction(Lexer lexer)
        {
            lexer.IgnoreAnyRun(" \t");

            char c = lexer.Peek();
            while (Char.IsLetter(c))
            {
                lexer.Next();
                c = lexer.Peek();
            }

            // Checking if it's a label (ie. 'MyLabel:'):
            if (lexer.Peek() == ':')
            {
                return LabelDefinition;
            }

            lexer.Emit(TokenType.Instruction);

            if (lexer.IsEOF()) return null;

            return Parameter;
        }

        internal static StateFunc LabelDefinition(Lexer lexer)
        {
            lexer.Emit(TokenType.LabelDefinition);
            lexer.IgnoreAnyRun(":");
            return Start;
        }

        // Checking whether the next string part is a label usage,
        // like: @MyLabel - Labels always start with a @-char.
        internal static StateFunc Label(Lexer lexer)
        {
            int consumed = 0;

            lexer.IgnoreAnyRun("@");
            while (true)
            {
                char c = lexer.Peek();
                if (!Char.IsLetter(c))
                {
                    if (consumed < 1)
                    {
                        return lexer.Error("Error: Label name must have at least one character: '" + lexer.Peek() + "' at pos: " + lexer.GetPos());
                    }
                    lexer.Emit(TokenType.Label);
                    return Start;
                }

                consumed += 1;
                lexer.Next();
            }
        }

        internal static StateFunc Parameter(Lexer lexer)
        {
            // Ignorning any whitespace:
            lexer.IgnoreAnyRun(" \t");

            char c = lexer.Peek();
            if (!Char.IsDigit(c))
            {
                // If the next char is a @ then 
                // it's going to be a label probably:
                if (c == '@')
                {
                    return Label;
                }

                return Start;
            }

            var digits = "0123456789";
            lexer.AcceptAnyRun(digits);
            lexer.Emit(TokenType.Parameter);

            return Parameter;
        }

        internal static StateFunc Comment(Lexer lexer)
        {
            while(!lexer.IsEOF() && lexer.Peek() != '\n')
            {
                lexer.Next();
            }

            lexer.Emit(TokenType.Comment);
            return Start;
        }

    }
}
