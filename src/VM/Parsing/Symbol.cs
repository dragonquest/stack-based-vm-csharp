namespace VM.Parsing
{
    public enum SymbolType {
        Label,
    }

    public class Symbol
    {
        public string Name;
        public SymbolType Type;
        public int Address;

        public Symbol(string name, SymbolType type, int address)
        {
           Name = name;
           Type = type;
           Address = address;
        }
    }
}
