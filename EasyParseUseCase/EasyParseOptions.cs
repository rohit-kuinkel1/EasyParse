//AUTO GENERATED USING https://github.com/rohit-kuinkel1/EasyParse/tree/main
using EasyParser.Core;
namespace Program.Parsing
{
    //add --read "File Name.txt" --verbose True --stdin false --Count 10
    //a -r "File Name.txt" -v True --s false --c 10
    [Verb('a', "add", Required = false, HelpText = "Add file contents to the index.")]
    public class ParseVerbs
    {
        [Options('r', "read", Default = null, Required = false, HelpText = "Input files to be processed.", Aliases = new[] { "reading", "studying" })] //Aliases must be strings with len >= 2
        [Settings( RegexPattern = @"^[a-zA-Z0-9\s]+\.(txt|doc|pdf)$", RegexOnFailureMessage = "File must have a valid extension (txt, doc, or pdf)" )]
        [Mutual(EasyParser.Enums.MutualType.Inclusive, nameof(Verbose), nameof(Stdin))]
        public string? InputFile { get; set; }

        [Options('v', "verbose", Default = false, Required = true, HelpText = "Prints all messages to standard output.")]
        [Mutual( EasyParser.Enums.MutualType.Exclusive, nameof( Count ))]
        public bool Verbose { get; set; }

        [Options('s', "stdin", Default = false, Required = false, HelpText = "Read from stdin", Aliases = new[] { "standardin", "stdinput" })]
        public bool Stdin { get; set; }

        [Options('c', "count", Default = 0, Required = false, HelpText = "Count of verbs", Aliases = new[] { "length", "total" })]
        [Settings( MaxValue = 20, MinValue = 0 )]
        public int Count { get; set; }
    }
  //public static void Main(string[] args)
    //{
        //LogLevel.BackTrace cannot be set by the users; levels => Debug, Info, Warning, Error, Critical, None
        //you can disable the EasyParser logger by: EasyParser.Logger.IsLoggerEnabled = false;
        //var parser = new EasyParse(LogLevel.Debug, false);
        //var parsingResult = parser.Parse<ParseVerbs>(args);
        //if(parsingResult.Success)
        //{
            // do something
        //}
        //else
        //{
            // do something
            // Console.WriteLine(parsingResult.ErrorMessage);
        //}
    //}
}