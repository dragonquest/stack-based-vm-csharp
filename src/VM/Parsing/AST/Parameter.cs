namespace VM.Parsing.AST
{
    public enum ParameterType
    {
        Label,
        Integer,
    }

    public class Parameter : Node
    {
        public ParameterType Type;

        public int IntegerValue;
        public string StringValue;
    }
}
