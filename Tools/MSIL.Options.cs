using System;

using CommandLine;
using CommandLine.Text;


public partial class MSIL
{
    private class Options
    {
        [Option('s', "source", Required = true, HelpText = "Input instructions file.")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output name of executable")]
        public string OutputFile { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText {
                Heading = new HeadingInfo("EasyVM - MSIL Compiler", "1.0"),
                        AdditionalNewLineAfterOption = true,
                        AddDashesToOption = true
            };
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine("The EasyVM MSIL Compiler takes an instruction file and creates a .NET executable.");
            help.AddPreOptionsLine(" ");
            help.AddPostOptionsLine("Usage: ./MSIL.exe --source file.vm --output file.exe");
            help.AddPostOptionsLine(" ");
            help.AddOptions(this);
            return help;
        }
    }
}
