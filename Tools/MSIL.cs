using System;

using Util.Exceptions;

using CommandLine;

public partial class MSIL
{
	static void Main(string[] args)
    {
		var options = new Options();
		if (!CommandLine.Parser.Default.ParseArguments(args, options))
		{
            Environment.Exit(1);
		}

        var compiler = new Compiler();
        compiler.Compile(options.InputFile, options.OutputFile);
    }
}
