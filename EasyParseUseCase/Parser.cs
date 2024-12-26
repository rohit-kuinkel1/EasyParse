using EasyParser.Core;

namespace Program
{
    /// <summary>
    /// <para>
    /// The demo <see cref="ParseVerbs"/> class encapsulates the command-line options for the EasyParse library. 
    /// The class name does not necessarily always have to be <see cref="ParseVerbs"/>; it is up to you to decide the name.
    /// Whatever the name, you need to provide it as a generic argument to <see cref="EasyParser.EasyParse.Parse{T}(string[])"/>.
    /// <see cref="ParseVerbs"/> is used in conjunction with the <see cref="VerbAttribute"/> to define verbs and their associated options.
    /// </para>
    /// <para>
    /// <see cref="MutualAttribute"/> denotes the mutual relationship between two <see cref="OptionsAttribute"/>.
    /// The enums <see cref="EasyParser.Enums.MutualType"/> can be used to denote a 
    /// <see cref="EasyParser.Enums.MutualType.Exclusive"/> or 
    /// <see cref="EasyParser.Enums.MutualType.Inclusive"/> relationship between two <see cref="OptionsAttribute"/>.
    /// If an attribute is marked to be mutually inclusive to another attribute, then the mutual relationship takes precedence over the <see cref="OptionsAttribute.Required"/>
    /// property, meaning even if an attribute is set to <see cref="OptionsAttribute.Required"/> = <see langword="false"/> but is mentioned as a mutual attribute
    /// to another attribute, the <see cref="OptionsAttribute.Required"/> for that particular option will still be interpreted as being set to 
    /// <see cref="OptionsAttribute.Required"/> = <see langword="true"/>.
    /// This is particularly useful for instances where 2 entities aren't necessarily required, but if they are, they are required together.
    /// </para>
    /// </summary>
    [Verb( 'a', "add", Required = false, HelpText = "Add file contents to the index." )]
    public class ParseVerbs
    {
        [Options( 'r', "read", Default = null, Required = false, HelpText = "Input files to be processed.", Aliases = new[] { "reading", "studying" } )]
        [Settings(  RegexPattern = @"^[a-zA-Z0-9\s]+\.(txt|doc|pdf)$", RegexOnFailureMessage = "File must have a valid extension (txt, doc, or pdf)" )]
        [Mutual( EasyParser.Enums.MutualType.Inclusive, nameof( Count ), nameof( Stdin ) )]
        public string? InputFile { get; set; }

        [Mutual( EasyParser.Enums.MutualType.Exclusive, "TEST")]
        [Options( 'v', "verbose", Default = false, Required = true, HelpText = "Prints all messages to standard output." )]
        public bool Verbose { get; set; }

        [Options( 's', "stdin", Default = false, Required = false, HelpText = "Read from stdin", Aliases = new[] { "standardin", "stdinput" } )]
        public bool Stdin { get; set; }

        [Options( 'c', "count", Default = 0, Required = false, HelpText = "Count of verbs", Aliases = new[] { "length", "total" } )] //Aliases have to be at least 2 stringed words
        public int Count { get; set; }
    }
}