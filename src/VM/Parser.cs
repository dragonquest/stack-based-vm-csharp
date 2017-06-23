using System;
using System.Collections.Generic;

using VM.Parsing;
using VM.Parsing.AST;
using VM.Util;

namespace VM
{
    public class Parser
    {
        // TryParse parses the source string and returns a list of errors on failure.
        // On success it fills the tree out parameter with the parsed tree.
        public static List<IError> TryParse(string source, out Tree tree)
        {
            // FIXME(an): Add try-catch block & convert to IError,
            // since Try* variants are supposed to be exception safe
            var lexer = new Lexer();
            lexer.Run(source);

            var sa = new SyntacticAnalyzer();
            tree = sa.Run(lexer.GetTokens());
            if (sa.GetErrors().Count > 0)
            {
                return sa.GetErrors();
            }

            var treeWalker = new Walker();

            var semanticAnalyzer = new SemanticAnalyzer();
            treeWalker.Walk(semanticAnalyzer, tree);
            if (semanticAnalyzer.GetErrors().Count > 0)
            {
                return semanticAnalyzer.GetErrors();
            }

            return new List<IError>();
        }
    }
}
