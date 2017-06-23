using System.Collections.Generic;

namespace VM.Parsing
{
    // SymbolTable inspired by:
    // https://en.wikipedia.org/wiki/Symbol_table
    // See: "Another table format"
    public class SymbolTable
    {
        private List<Symbol> _entries;

        public SymbolTable()
        {
            _entries = new List<Symbol>();
        }

        public void Add(Symbol sym)
        {
            _entries.Add(sym);
        }

        public Symbol Lookup(SymbolType type, string name)
        {
            foreach (var sym in _entries)
            {
                if (type == sym.Type && name == sym.Name)
                {
                    return sym;
                }
            }

            return null;
        }
    }
}
