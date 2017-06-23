using Context = VM.Interpreter.Context;

namespace VM
{
    public interface IDebugger
    {
        Context BeforeInstruction(Context ctx);
        Context AfterInstruction(Context ctx);
    }
}

