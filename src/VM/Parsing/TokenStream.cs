using System.Collections.Generic;

namespace VM.Parsing
{
    public class TokenStream
    {
        private readonly List<Item> _tokens;
        private int _pos;

        public TokenStream(List<Item> toks)
        {
            _tokens = toks;
            _pos = 0;
        }

        public bool HasMore()
        {
            return _pos < _tokens.Count;
        }

        public Item Next()
        {
            var item = _tokens[_pos];
            _pos++;
            return item;
        }

        public Item Peek()
        {
            var item = Next();
            Backup();
            return item;
        }

        public void Backup()
        {
            _pos--;
        }
    }
}
