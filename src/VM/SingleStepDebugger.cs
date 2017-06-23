using System;
using System.Collections.Generic;

using Context = VM.Interpreter.Context;

namespace VM
{
    public class SingleStepDebugger : IDebugger
    {
        private readonly IDebugger _tracer;

        public SingleStepDebugger()
        {
            _tracer = new Tracer();
        }

        public Context BeforeInstruction(Context ctx)
        {
            return _tracer.BeforeInstruction(ctx);
        }

        public Context AfterInstruction(Context ctx)
        {
            ctx = _tracer.AfterInstruction(ctx);

            Console.Write("(debugger) ");
            var line = Console.ReadLine();

            switch (line.ToLower())
            {
                case "next":
                case "n":
                default:
                    return ctx;
            }
        }
    }
}

