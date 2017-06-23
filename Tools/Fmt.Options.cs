using CommandLine;
using CommandLine.Text;

public partial class Fmt
{
    private class Options
    {
        [Option('s', "source", Required = true, HelpText = "Input instructions file.")]
        public string InputFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText {
                Heading = new HeadingInfo("EasyVM - Formatter", "1.0"),
                                            AdditionalNewLineAfterOption = true,
                                            AddDashesToOption = true
            };
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine("The EasyVM Formatter takes an instruction file and prints it correctly formatted to stdout.");
            help.AddPreOptionsLine(" ");
            help.AddPostOptionsLine("Usage: ./Fmt.exe --source file.vm");
            help.AddPostOptionsLine(" ");
            help.AddOptions(this);
            return help;
        }
    }

}
