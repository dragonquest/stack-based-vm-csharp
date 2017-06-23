using System;
using System.IO;
using System.Collections.Generic;

using VM.Parsing.AST;
using VM.Util;

public partial class Fmt
{
    private class Formatter
    {
        public Formatter() {}

        public Tuple<string, List<IError>> Format(string inputFile)
        {
            string source = File.ReadAllText(inputFile);

            VM.Parsing.AST.Tree tree;
            var errors = VM.Parser.TryParse(source, out tree);
            if (errors.Count > 0)
            {
                return new Tuple<string, List<IError>>("", errors);
            }

            var treeWalker = new Walker();
            var fmt = new PrettyPrinter();
            treeWalker.Walk(fmt, tree);

            return new Tuple<string, List<IError>>(fmt.GetOutput(), new List<IError>());
        }
    }
}
