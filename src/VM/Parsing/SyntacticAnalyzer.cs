using System;
using System.Collections.Generic;

using VM.Parsing.AST;
using VM.Util;

namespace VM.Parsing
{
    // The SyntacticAnalyzer checks if the tokens are arranged in the
    // correct order and also builds the parse tree / syntax tree
    public class SyntacticAnalyzer
    {
        private List<IError> _errors;

        public SyntacticAnalyzer()
        {
            _errors = new List<IError>();
        }

        public Tree Run(List<Item> items)
        {
            var tree = new Tree();

            var tokens = new TokenStream(items);

            while (tokens.HasMore())
            {
                var curTok = tokens.Peek();
                switch (curTok.Type)
                {
                    case TokenType.Comment:
                        {
                            var line = new Line(LineType.Comment, curTok.Value);
                            line.LineNumber = curTok.Line;

                            tree.Line.Add(line);
                            tokens.Next();
                        }
                        break;

                    case TokenType.LabelDefinition:
                        {
                            tokens.Next();
                            var line = new Line(LineType.Label, curTok.Value);
                            line.LineNumber = curTok.Line;

                            maybeAddComment(curTok.Line, ref line, ref tokens);
                            tree.Line.Add(line);
                        }
                        break;

                    case TokenType.Instruction:
                        {
                            var ins = new Instruction(tokens.Next().Value);
                            ins.LineNumber = curTok.Line;

                            while (tokens.HasMore())
                            {
                                var param = new Parameter();

                                if (tokens.Peek().Type == TokenType.Parameter)
                                {
                                    var paramTok = tokens.Next();

                                    param.Type = ParameterType.Integer;
                                    param.IntegerValue = Convert.ToInt32(paramTok.Value);
                                    param.LineNumber = curTok.Line;

                                    ins.Parameters.Add(param);
                                }
                                else if (tokens.Peek().Type == TokenType.Label)
                                {
                                    var labelTok = tokens.Next();

                                    param.Type = ParameterType.Label;
                                    param.StringValue = labelTok.Value;
                                    param.LineNumber = curTok.Line;

                                    ins.Parameters.Add(param);
                                }
                                else {
                                    break;
                                }
                            }

                            var line = new Line(LineType.Instruction, ins);
                            line.LineNumber = curTok.Line;

                            maybeAddComment(curTok.Line, ref line, ref tokens);

                            tree.Line.Add(line);
                        }
                        break;

                    default:
                        _errors.Add(new Error(String.Format("Got type {0} ({1}) on line {2} but expected either an instruction or a comment", curTok.Type, curTok.Value, curTok.Line)));
                        return tree;
                }
            }

            return tree;
        }

        public List<IError> GetErrors()
        {
            return _errors;
        }

        private void maybeAddComment(int currentLine, ref Line line, ref TokenStream tokens)
        {
            // Maybe there's a comment at the end of the instruction:
            if (tokens.HasMore() &&
                tokens.Peek().Type == TokenType.Comment &&
                currentLine == tokens.Peek().Line)
            {
                var comment = tokens.Next();
                line.Comment = comment.Value;
            }
        }
    }
}
