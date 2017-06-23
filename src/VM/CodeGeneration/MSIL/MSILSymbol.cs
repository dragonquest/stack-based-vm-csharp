using System.Reflection.Emit;

using VM.Parsing;

namespace VM.CodeGeneration.MSIL
{
    // Extending the Symbol with a MSIL Label.
    // When generating MSIL code it is necessary
    // that all used labels are declared upfront.
    public class MSILSymbol : Symbol
    {
        public readonly Label Label;

        public MSILSymbol(string name, SymbolType type, int address, Label label) : base(name, type, address)
        {
           Label = label;
        }
    }
}
