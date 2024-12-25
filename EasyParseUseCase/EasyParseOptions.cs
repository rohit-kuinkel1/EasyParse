//AUTO GENERATED USING EASY PARSER https://github.com/rohit-kuinkel1/EasyParse/tree/main
using EasyParser.Core;
namespace Program.Parsing
{
    [Verb('a', "add", Required = false, HelpText = "Add file contents to the index.")]
    public class ParseVerbs
    {
        [Options('r', "read", Default = null, Required = false, HelpText = "Input files to be processed.", Aliases = new[] { "reading", "studying" })]
        [Mutual(EasyParser.Enums.MutualType.Inclusive, nameof(Verbose), nameof(Stdin))]
        public string? InputFile { get; set; }

        [Options('v', "verbose", Default = false, Required = true, HelpText = "Prints all messages to standard output.")]
        [Mutual( EasyParser.Enums.MutualType.Exclusive, nameof( Count ))]
        public bool Verbose { get; set; }

        [Options('s', "stdin", Default = false, Required = false, HelpText = "Read from stdin", Aliases = new[] { "standardin", "stdinput" })]
        public bool Stdin { get; set; }

        [Options('c', "count", Default = 0, Required = false, HelpText = "Count of verbs", Aliases = new[] { "length", "total" })]
        public int Count { get; set; }
    }
}