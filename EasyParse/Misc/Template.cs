namespace EasyParse.Misc
{
    internal static class Template
    {
        internal static readonly string templateFileName = "EasyParseOptions.cs";
        internal static string GetConfigTemplateWithMain()
        {
            return Template.GetConfigTemplateBase() + "\n" + Template.GetMainMethodTemplate() + "\n}";
        }

        internal static string GetConfigTemplateWithoutMain()
        {
            return Template.GetConfigTemplateBase() + "\n}";
        }

        /// <summary>
        ///  Gets the config template string including the public static void Main(string[] args)... part
        /// </summary>
        /// <returns></returns>
        public static string GetConfigTemplateBase()
        {
            return
        @"//AUTO GENERATED USING https://github.com/rohit-kuinkel1/EasyParse/tree/main
using EasyParser.Core;
namespace Program
{
    /// <summary>
    /// <para>
    /// The demo <see cref=""ParseVerbs""/> class encapsulates the command-line options for the EasyParse library. 
    /// The class name does not necessarily always have to be <see cref=""ParseVerbs""/>; it is up to you to decide the name.
    /// Whatever the name, you need to provide it as a generic argument to <see cref=""EasyParser.EasyParse.Parse{T}(string[])""/>.
    /// <see cref=""ParseVerbs""/> is used in conjunction with the <see cref=""VerbAttribute""/> to define verbs and their associated options.
    /// Meaning, if a class is to be defined as a verb, then it must be decorated with the attribute <see cref=""VerbAttribute""/>, just like
    /// how its demonstrated below.
    /// </para>
    /// <para>
    /// <see cref=""MutualAttribute""/> denotes the mutual relationship between two <see cref=""OptionsAttribute""/>.
    /// The enums <see cref=""EasyParser.Enums.MutualType""/> can be used to denote a 
    /// <see cref=""EasyParser.Enums.MutualType.Exclusive""/> or 
    /// <see cref=""EasyParser.Enums.MutualType.Inclusive""/> relationship between two <see cref=""OptionsAttribute""/>.
    /// If an attribute is marked to be mutually inclusive to another attribute, then the mutual relationship takes precedence over the <see cref=""OptionsAttribute.Required""/>
    /// property, meaning even if an attribute is set to <see cref=""OptionsAttribute.Required""/> = <see langword=""false""/> but is mentioned as a mutual attribute
    /// to another attribute, the <see cref=""OptionsAttribute.Required""/> for that particular option will still be interpreted as being set to 
    /// <see cref=""OptionsAttribute.Required""/> = <see langword=""true""/>.
    /// This is particularly useful for instances where 2 entities aren't necessarily required, but if they are, they are required together.
    /// </para>
    /// <para>
    /// <see cref=""SettingsAttribute""/> can be used to put constraints on an option.
    /// For instance, the <see cref=""Count""/> property of this class <see cref=""ParseVerbs""/> takes in int values, but the value that it can take are
    /// limited to [0,20]. If the user provided value does not lie in the defined range, then the parsing was not successful and <see cref=""EasyParser.EasyParse.Parse{T}(string[])""/>
    /// returns with <see langword=""false""/>.
    /// Another instance for <see cref=""SettingsAttribute""/> is <see cref=""InputFile""/> where it has be constrainted with a RegexPattern where the user provided value can only end with txt,doc or pdf.
    /// </para>
    /// </summary>
    //add --read ""File Name.txt"" --verbose True --stdin false --Count 10
    //add where read is ""File Name.txt"" verbose is True stdin is false count is 10
    //a -r ""File Name.txt"" -v True -s false -c 10
    [Verb('a', ""add"", Required = false, HelpText = ""Add file contents to the index."")]
    public class ParseVerbs
    {
        [Options('r', ""read"", Default = null, Required = false, HelpText = ""Input files to be processed."", Aliases = new[] { ""reading"", ""studying"" })] //Aliases must be strings with len >= 2
        [Settings( RegexPattern = @""^[a-zA-Z0-9\s]+\.(txt|doc|pdf)$"", RegexOnFailureMessage = ""File must have a valid extension (txt, doc, or pdf)"" )]
        [Mutual(EasyParser.Enums.MutualType.Inclusive, nameof(Verbose), nameof(Stdin))]
        public string? InputFile { get; set; }

        [Options('v', ""verbose"", Default = false, Required = true, HelpText = ""Prints all messages to standard output."")]
        [Mutual( EasyParser.Enums.MutualType.Exclusive, nameof( Count ))]
        public bool Verbose { get; set; }

        [Options('s', ""stdin"", Default = false, Required = false, HelpText = ""Read from stdin"", Aliases = new[] { ""standardin"", ""stdinput"" })]
        public bool Stdin { get; set; }

        [Options('c', ""count"", Default = 0, Required = false, HelpText = ""Count of verbs"", Aliases = new[] { ""length"", ""total"" })]
        [Settings( MaxValue = 20, MinValue = 0 )]
        public int Count { get; set; }
    }";
        }

        public static string GetMainMethodTemplate()
        {
            return
@"  
    //public static void Main(string[] args)
    //{
        ////LogLevel.BackTrace cannot be set by the users; levels => Debug, Info, Warning, Error, Critical, None

        //var parser = new EasyParser.EasyParse( minLogLevel: EasyParser.LogLevel.Debug, redirectLogsToFile: false );
        // parser.SetLoggerStatusEnabled( true );

        //var parsingResult = parser.Parse<ParseVerbs>(args);
        //if(parsingResult.Success)
        //{
            //// do something
        //}
        //else
        //{
            //// do something
            // Console.WriteLine(parsingResult.ErrorMessage);
        //}
    //}";
        }
    }
}
