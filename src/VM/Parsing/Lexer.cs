using System;
using System.Collections.Generic;

namespace VM.Parsing
{
    public enum TokenType
    {
        Instruction,
        Parameter,
        Comment,
        Error,
        LabelDefinition,
        Label, // Label usage such as @labelname
    }

    public struct Item
    {
        public TokenType Type;
        public string Value;

        public int Line;
    }

    // Lexer StateFunc delegate is used to implement
    // the LexFunc's. See LexFunc.cs
    public delegate StateFunc StateFunc(Lexer lexer);

    // The Lexer hashes the (string) input into tokens
    public class Lexer
    {
        public const char EOF = unchecked((char)-1);

        private string _input; // the string being scanned
        private int _start; // start position of this item
        private int _pos; // current position of input
        private int _line; // current line

        private List<Item> _items;

        public Lexer()
        {
            // FIXME(an): When trying to create a helper
            // function and call it here and in Run()
            // then the code does not seem to compile and work
            // figure out how to solve this in a better way to
            // avoid duplication.
            _input = "";
            _start = 0;
            _pos = 0;
            _line = 1;
            _items = new List<Item>();
        }

        public List<Item> GetTokens()
        {
            return _items;
        }

        public void Run(string input)
        {
            _input = input;
            _start = 0;
            _pos = 0;
            _line = 1;
            _items = new List<Item>();

            try
            {
                StateFunc state = LexFunc.Start(this);
                while (state != null)
                {
                    state = state(this);
                }
            }
            catch (Exception ex)
            {
                Error(String.Format("Exception occured: {0}", ex));
            }
        }

        internal int GetPos()
        {
            return _pos;
        }

        // Error emits the current input as an Error-Token.
        internal StateFunc Error(string msg)
        {
            _items.Add(new Item() {
                Type = TokenType.Error,
                Value = msg,
                Line = _line,
            });

            return null;
        }

        internal bool Emit(TokenType t)
        {
            if (_start >= _pos || _start >= _input.Length)
            {
                return false;
            }

            _items.Add(new Item() {
                Type = t,
                Value = _input.Substring(_start, _pos - _start),
                Line = _line,
            });

            _start = _pos;
            return true;
        }

        internal void Ignore()
        {
            _start = _pos;
        }

        internal bool IsEOF()
        {
            return _pos >= _input.Length;
        }

        internal char Next()
        {
            if (IsEOF())
            {
                return EOF;
            }

            char c = _input[_pos];
            _pos += 1;

            if (c == '\n') _line += 1;

            return c;
        }

        internal void Backup()
        {
            _pos -= 1;

            if (_input[_pos] == '\n') _line -= 1;
        }

        internal char Peek()
        {
            if (IsEOF())
            {
                return EOF;
            }

            var c = Next();
            Backup();
            return c;
        }

        internal void IgnoreAnyRun(string chars)
        {
            AcceptAnyRun(chars);
            Ignore();
        }

        internal bool AcceptAny(string chars)
        {
            if (chars.IndexOf(Next()) >= 0)
            {
                return true;
            }

            if (Peek() == EOF)
            {
                return false;
            }

            Backup();
            return false;
        }

        internal void AcceptAnyRun(string chars)
        {
            while(chars.IndexOf(Next()) >= 0)
            {
            }

            if (Peek() == EOF)
            {
                return;
            }

            Backup();
        }
    }
}
