//AUTO GENERATED USING https://github.com/rohit-kuinkel1/EasyParse/tree/main
using EasyParser.Core;
namespace Program
{
    /// <summary>
    /// <para>
    /// The demo <see cref="ParseVerbs"/> class encapsulates the command-line options for the EasyParse library. 
    /// The class name does not necessarily always have to be <see cref="ParseVerbs"/>; it is up to you to decide the name.
    /// Whatever the name, you need to provide it as a generic argument to <see cref="EasyParser.EasyParse.Parse{T}(string[])"/>.
    /// <see cref="ParseVerbs"/> is used in conjunction with the <see cref="VerbAttribute"/> to define verbs and their associated options.
    /// Meaning, if a class is to be defined as a verb, then it must be decorated with the attribute <see cref="VerbAttribute"/>, just like
    /// how its demonstrated below.
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
    /// <para>
    /// <see cref="SettingsAttribute"/> can be used to put constraints on an option.
    /// For instance, the <see cref="Count"/> property of this class <see cref="ParseVerbs"/> takes in int values, but the value that it can take are
    /// limited to [0,20]. If the user provided value does not lie in the defined range, then the parsing was not successful and <see cref="EasyParser.EasyParse.Parse{T}(string[])"/>
    /// returns with <see langword="false"/>.
    /// Another instance for <see cref="SettingsAttribute"/> is <see cref="InputFile"/> where it has be constrainted with a RegexPattern where the user provided value can only end with txt,doc or pdf.
    /// </para>
    /// </summary>
    //add --read "File Name.txt" --verbose True --stdin false --Count 10
    //add where read is "File Name.txt" verbose is True stdin is false count is 10
    //a -r "File Name.txt" -v True -s false -c 10
    [Verb('a', "add", Required = false, HelpText = "Add file contents to the index.")]
    public class ParseVerbs
    {
        [Options('r', "read", Default = null, Required = true, HelpText = "Input files to be processed.", Aliases = new[] { "reading", "studying" })] //Aliases must be strings with len >= 2
        [Settings( RegexPattern = @"^[a-zA-Z0-9\s]+\.(txt|doc|pdf)$", RegexOnFailureMessage = "File must have a valid extension (txt, doc, or pdf)" )]
        [Mutual(EasyParser.Enums.MutualType.Inclusive, nameof(Verbose), nameof(Stdin))]
        public string? InputFile { get; set; }

        [Options('v', "verbose", Default = false, Required = true, HelpText = "Prints all messages to standard output.")]
        //[Mutual( EasyParser.Enums.MutualType.Exclusive, nameof( Count ))]
        public bool Verbose { get; set; }

        [Options('s', "stdin", Default = false, Required = false, HelpText = "Read from stdin", Aliases = new[] { "standardin", "stdinput" })]
        public bool Stdin { get; set; }

        [Options('c', "count", Default = 0, Required = false, HelpText = "Count of verbs", Aliases = new[] { "length", "total" })]
        [Settings( MaxValue = 20, MinValue = 0 )]
        public int Count { get; set; }
    }

    [Verb( 'p', "process", Required = false, HelpText = "Process video files with specified parameters." )]
    public class VideoProcessVerbs
    {
        [Options( 'i', "input", Default = null, Required = true, HelpText = "Input video file path.", Aliases = new[] { "source", "videofile" } )]
        [Settings( RegexPattern = @"^[a-zA-Z0-9\s]+\.(mp4|avi|mov)$", RegexOnFailureMessage = "File must be a valid video format (mp4, avi, or mov)" )]
        [Mutual( EasyParser.Enums.MutualType.Inclusive, nameof( Quality ), nameof( Duration ) )]
        public string? VideoPath { get; set; }

        [Options( 'q', "quality", Default = "720p", Required = false, HelpText = "Output video quality.", Aliases = new[] { "resolution", "format" } )]
        [Settings( AllowedValues = new[] { "480p", "720p", "1080p", "4k" } )]
        public string? Quality { get; set; }

        [Options( 'd', "duration", Default = 0, Required = false, HelpText = "Maximum duration in seconds", Aliases = new[] { "length", "time" } )]
        [Settings( MaxValue = 3600, MinValue = 1 )]
        public int Duration { get; set; }

        [Options( 'f', "force", Default = false, Required = false, HelpText = "Force overwrite existing files" )]
        public bool Force { get; set; }
    }

    [Verb( 'b', "backup", Required = false, HelpText = "Backup database with specified options." )]
    public class DatabaseBackupVerbs
    {
        [Options( 'h', "host", Default = "localhost", Required = true, HelpText = "Database host address.", Aliases = new[] { "server", "address" } )]
        [Settings( RegexPattern = @"^([a-zA-Z0-9.-]+|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})$", RegexOnFailureMessage = "Invalid host format" )]
        [Mutual( EasyParser.Enums.MutualType.Inclusive, nameof( Port ), nameof( Compress ) )]
        public string? Host { get; set; }

        [Options( 'p', "port", Default = 5432, Required = false, HelpText = "Database port number", Aliases = new[] { "portnumber" } )]
        [Settings( MaxValue = 65535, MinValue = 1 )]
        public int Port { get; set; }

        [Options( 'c', "compress", Default = false, Required = false, HelpText = "Enable backup compression" )]
        public bool Compress { get; set; }

        [Options( 'e', "encrypt", Default = false, Required = false, HelpText = "Enable encryption" )]
        [Mutual( EasyParser.Enums.MutualType.Inclusive, nameof( Compress ) )]
        public bool Encrypt { get; set; }
    }

    [Verb( 'w', "weather", Required = false, HelpText = "Configure weather station parameters." )]
    public class WeatherStationVerbs
    {
        [Options( 'l', "location", Default = null, Required = true, HelpText = "Station location coordinates.", Aliases = new[] { "coordinates", "position" } )]
        [Settings( RegexPattern = @"^-?\d+\.?\d*,\s*-?\d+\.?\d*$", RegexOnFailureMessage = "Location must be in format: latitude,longitude" )]
        [Mutual( EasyParser.Enums.MutualType.Inclusive, nameof( Interval ), nameof( Units ) )]
        public string? Location { get; set; }

        [Options( 'i', "interval", Default = 5, Required = false, HelpText = "Measurement interval in minutes", Aliases = new[] { "frequency", "period" } )]
        [Settings( MaxValue = 60, MinValue = 1 )]
        public int Interval { get; set; }

        [Options( 'u', "units", Default = "metric", Required = false, HelpText = "Measurement units system", Aliases = new[] { "system", "measurements" } )]
        [Settings( AllowedValues = new[] { "metric", "imperial" } )]
        public string? Units { get; set; }

        [Options( 'o', "offline", Default = false, Required = false, HelpText = "Enable offline mode" )]
        public bool Offline { get; set; }
    }
}