using System;
using System.Collections.Generic;

using Context = VM.Interpreter.Context;

namespace VM
{
    public class Tracer : IDebugger
    {
        private readonly Bytecode _bytecode;

        public Tracer()
        {
            _bytecode = new Bytecode();
        }

        public Context BeforeInstruction(Context ctx)
        {
            var opcode = ctx.Code[ctx.IP];

            Console.Error.Write("{0:0000}: {1,-10} {2}\t\t", ctx.IP, _bytecode.FindNameByOpcode(opcode), String.Join(", ", buildArgsList(ctx, _bytecode.GetNumArgs(opcode))));
            return ctx;
        }

        public Context AfterInstruction(Context ctx)
        {
            Console.Error.Write("[SP: {0}] \t", ctx.SP);
            Console.Error.Write("| Stack: {0}", String.Join(", ", buildStackTrace(ctx).ToArray()));
            Console.Error.Write("\n");
            return ctx;
        }
        
        private List<int> buildArgsList(Context ctx, int numArgs)
        {
            var args = new List<int>();

            for (int i = ctx.IP+1; i <= ctx.IP + numArgs; i++)
            {
                args.Add(ctx.Code[i]);
            }
            return args;
        }

        private List<int> buildStackTrace(Context ctx)
        {
            var stackTrace = new List<int>();

            if (ctx.SP < 0)
            {
                return stackTrace;
            }

            for (int i = 0; i <= ctx.SP; i++)
            {
                stackTrace.Add(ctx.Stack[i]);
            }
            return stackTrace;
        }
    }
}

