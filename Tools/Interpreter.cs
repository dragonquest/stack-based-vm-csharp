using System;
using System.Collections.Generic;

using VM;
using Util.Exceptions;

using CommandLine;

public partial class Interpreter
{
	static void Main(string[] args)
    {
		var options = new Options();
		if (!CommandLine.Parser.Default.ParseArguments(args, options))
		{
            Environment.Exit(1);
		}

        var compiler = new BytecodeCompiler();
        ExceptionOr<List<int>> result = Safe.Wrapper(compiler).FileToInstructions(options.InputFile);
        if (result.HasFailed())
        {
            Console.Error.WriteLine("Failed to compile the instructions file: {0}", result.GetException().Message);
            Environment.Exit(2);
        }

        if (options.ShowOpcodes)
        {
            foreach (var op in result.GetValue())
            {
                Console.Write("{0} ", op);
            }
            Console.WriteLine();
            return;
        }

        if (options.ShowLayout)
        {
            var bytecode = new Bytecode();
            int address = 0;

            int length = result.GetValue().Count;
            var opcodes = result.GetValue();
            int i = 0;
            while (i < length)
            {
                var insName = bytecode.FindNameByOpcode(opcodes[i]);
                var numArgs = bytecode.GetNumArgs(opcodes[i]);

                Console.Write("{0:0000}:\t{1} ", i, insName);
                i++;
                for (int j = 0; j < numArgs; j++)
                {
                    Console.Write("{0} ", opcodes[i]);
                    i++;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            return;
        }

        var interpreter = new VM.Interpreter();
        if (options.Debugger != null)
        {
            var monitor = new Monitor();
            monitor.InsertNewLineBefore = true;
            interpreter.SetMonitor(monitor);

            switch (options.Debugger)
            {
                case "tracer":
                    interpreter.Attach(new VM.Tracer());
                    break;

                case "singlestep":
                    interpreter.Attach(new VM.SingleStepDebugger());
                    break;

                default:
                    Console.Error.WriteLine("\"{0}\" is not a valid debugger. Use: \"tracer\", \"singlestep\"", options.Debugger);
                    Environment.Exit(3);
                    break;

            }
        }

        var ins = result.GetValue();
        if (ins.Count > 0)
        {
            interpreter.Run(result.GetValue(), 0);
        }
    }
}
