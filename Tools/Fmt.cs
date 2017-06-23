using System;
using System.Collections.Generic;

using VM.Util;
using Util.Exceptions;

using CommandLine;

public partial class Fmt
{
	static void Main(string[] args)
    {
		var options = new Options();
		if (!CommandLine.Parser.Default.ParseArguments(args, options))
		{
            Environment.Exit(1);
		}

        var formatter = new Formatter();
        ExceptionOr<Tuple<string, List<IError>>> result = Safe.Wrapper(formatter).Format(options.InputFile);
        if (result.HasFailed())
        {
            Console.Error.WriteLine("Failed to reformat the instructions file: {0}", result.GetException());
            Environment.Exit(2);
        }

        var output = result.GetValue().Item1;
        var errorList = result.GetValue().Item2;

        if (errorList.Count > 0)
        {
            foreach (var err in errorList)
            {
                Console.Error.WriteLine(err);
            }
            Environment.Exit(3);
        }

        Console.WriteLine("{0}", output);
    }
}

