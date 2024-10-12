using EasyParser.Core;

namespace Program
{
    public class ParseOptions
    {
        [Options( 'v', "verbose", HelpText = "Enable verbose output." )]
        public bool Verbose { get; set; }

        [Options( 'c', "config", HelpText = "Path to the config file." )]
        public string? ConfigPath { get; set; }
    }

    [Verb( 'a', "add", Required = false, HelpText = "Add file contents to the index." )]
    public class ParseVerbs
    {
        [Options( 'r', "read", Required = false, HelpText = "Input files to be processed." )]
        public string? InputFile { get; set; }

        [Options( 'v', "verbose", Default = false, HelpText = "Prints all messages to standard output." )]
        public bool Verbose { get; set; }

        [Options( 's',"stdin", Default = false, HelpText = "Read from stdin" )]
        public bool Stdin { get; set; }
    }
}