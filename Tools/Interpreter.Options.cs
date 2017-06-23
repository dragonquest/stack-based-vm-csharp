using System;

using CommandLine;
using CommandLine.Text;

public partial class Interpreter
{
    private class Options
    {
        [Option('s', "source", Required = true, HelpText = "Input instructions file.")]
        public string InputFile { get; set; }

        [Option('d', "attach debugger (\"tracer\", \"singlestep\")", Required = false, HelpText = "Enable tracing")]
        public string Debugger { get; set; }

        [Option('o', "opcodes", Required = false, HelpText = "Print the opcodes and exit.")]
        public bool ShowOpcodes { get; set; }
        
        [Option('l', "layout", Required = false, HelpText = "Print the layout and exit.")]
        public bool ShowLayout { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText {
              Heading = new HeadingInfo("EasyVM - Interpreter", "1.0"),
              AdditionalNewLineAfterOption = true,
              AddDashesToOption = true
            };
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine("The EasyVM Interpreter takes an instruction file and executes it.");
            help.AddPreOptionsLine(" ");
            help.AddPostOptionsLine("Usage: ./Interpreter.exe --source file.vm");
            help.AddPostOptionsLine(" ");
            help.AddOptions(this);
            return help;
        }
    }
}
