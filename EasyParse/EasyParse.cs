using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EasyParser.Enums;
using EasyParser.Parsing;
using EasyParser.Utility;

namespace EasyParser
{
    /// <summary>
    /// public entry point for the Parser.
    /// </summary>
    public class EasyParse
    {
        /// <summary>
        /// abstraction of the parsing type to parse the args
        /// </summary>
        private IParsing? _parsing;

        /// <summary>
        /// HashSet of reserved keywords during arg parsing.
        /// Contains all the values of the <see cref="EasyParser.Enums.ParsingKeyword"/> in string format.
        /// </summary>
        private static readonly HashSet<string> Keywords = new HashSet<string>(
            Enum.GetNames( typeof( ParsingKeyword ) )
                .Select( k => k.ToLowerInvariant() )
        );

        /// <summary>
        /// Default constructor for <see cref="EasyParse"/>
        /// </summary>
        public EasyParse()
        {
        }


        /// <summary>
        /// Parameterized Constructor for <see cref="EasyParse"/>.
        /// Set the <paramref name="minLogLevel"/> to the desired minimum logLevel.
        /// Set the <paramref name="redirectLogsToFile"/> to <see langword="true"/> if you want to redirect all the logs
        /// from <see cref="EasyParse"/> to a log file.
        /// <see cref="LogLevel.BackTrace"/> cannot be set by users.
        /// </summary>
        public EasyParse( LogLevel minLogLevel = LogLevel.Info, bool redirectLogsToFile = false )
        {
            Logger.Initialize( minLogLevel, redirectLogsToFile );
        }

        /// <summary>
        /// Parses the arguments provided to an instance of <see cref="EasyParse"/> and delegates the processing to the respective class.
        /// <para>
        /// If the natural language syntax was used, the keyword 'where' must be used at index 1 to denote that natural language has been used.
        /// Example: add where FilePath is ABCDEFG/HIJKLMNOP Contains is "photos, text, data" IsPasswordLocked is false
        /// </para>
        /// <para>
        /// If the conventional syntax was used, there is no need to use the 'where' keyword.
        /// Example: add --FilePath ABCDEFG/HIJKLMNOP --Contains "photos, text, data" --IsPasswordLocked false
        /// </para>
        /// Save the value returned by <see cref="Parse{T}(string[])"/> locally to use it.
        /// <code>
        /// //where <typeparamref name="T"/> is the class where the verbs and its related options are defined.
        /// var result = easyParser.Parse{<typeparamref name="T"/>}(args);
        /// if (result.Success)
        /// {
        ///     //do something
        /// }
        /// else
        /// {
        ///      Console.WriteLine( parsingResult.ErrorMessage );
        /// }
        /// </code>
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Instance of <see cref="ParsingResult{Type}"/> containing flag <see cref="bool"/> <see cref="ParsingResult{Type}.Success"/> which denotes if the parsing was successful or not.</returns>
        /// <exception cref="NullException"> When the provided <paramref name="args"/> was null.</exception>
        /// <exception cref="BadFormatException"> When the provided <paramref name="args"/> was badly formatted.</exception>
        /// <exception cref="IllegalOperation"> When type mismatch occurs or when static/abstract class is provided as type for instance.</exception>
        /// <exception cref="Exception"> For general unforseen exceptions.</exception>
        public ParsingResult<T> Parse<T>( string[] args ) where T : class, new()
        {
            try
            {
                _ = Utility.Utility.NotNullValidation( args, true );

                var isNaturalLanguage = args.Length > 1
                                        && string.Equals( args[1], ParsingKeyword.Where.ToString(), StringComparison.OrdinalIgnoreCase );

                var containsKeywords = args.Any( arg => Keywords.Contains( arg.ToLowerInvariant() ) );

                if( isNaturalLanguage && containsKeywords )
                {
                    Logger.BackTrace( "Using NaturalLanguageParsing" );
                    _parsing = new NaturalLanguageParsing();
                }
                else if( !containsKeywords )
                {
                    Logger.BackTrace( "Using StandardLanguageParsing" );
                    _parsing = new StandardLanguageParsing();
                }
                else
                {
                    throw new BadFormatException(
                        "Invalid structure for input args. Reserved keywords were detected to have been " +
                        "used for standard parsing. Please refrain from mixing natural language and standard language format and try again."
                    );
                }

                return _parsing.Parse<T>( args );
            }
            catch( NullException ex )
            {
                Logger.Critical( $"{ex.GetType()} {ex.Message}" );
                return new ParsingResult<T>( false, ex.Message, default! );
            }
            catch( Exception ex )
            {
                Logger.Critical( $"{ex.GetType()} {ex.Message}" );
                Logger.Critical( ex.StackTrace );
                return new ParsingResult<T>( false, ex.Message, default! );
            }
        }

        private static string? GetInitialDirectory()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if( entryAssembly == null )
            {
                Console.WriteLine( "Unable to determine the entry assembly." );
                return default;
            }

            var currentDirectory = Path.GetDirectoryName( entryAssembly.Location );
            if( currentDirectory == null )
            {
                Console.WriteLine( "Unable to determine the current directory." );
                return default;
            }

            return currentDirectory;
        }

        private static bool TestDirectoryWriteAccess( string directory )
        {
            try
            {
                var testFile = Path.Combine( directory, "test_write_permission.tmp" );
                File.WriteAllText( testFile, "test" );
                File.Delete( testFile );
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string FindWritableParentDirectory( string? startDirectory )
        {
            if( startDirectory == null )
            {
                Console.WriteLine( $"Param {nameof( startDirectory )} was null when it was not supposed to be. Cannot export default config" );
                return default;
            }

            var currentDirectory = startDirectory;
            for( int i = 0; i < 3; i++ )
            {
                var parentDirectory = Directory.GetParent( currentDirectory )?.FullName;
                if( parentDirectory == null )
                {
                    Console.WriteLine( $"Reached the root directory. Using current directory: {currentDirectory}" );
                    break;
                }

                if( TestDirectoryWriteAccess( parentDirectory ) )
                {
                    currentDirectory = parentDirectory;
                }
                else
                {
                    Console.WriteLine( $"Cannot access or write to parent directory. Using current directory: {currentDirectory}" );
                    break;
                }
            }

            return currentDirectory;
        }

        /// <summary>
        /// Saves the config provided in <paramref name="configContent"/> to path <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="configContent"></param>
        private static void SaveConfigFile( string directory, string configContent )
        {
            try
            {
                var finalPath = Directory.Exists( directory )
                    ? directory
                    : Directory.GetCurrentDirectory();

                var filePath = Path.Combine( directory, "EasyParseOptions.cs" );
                File.WriteAllText( filePath, configContent );
                Console.WriteLine( $"Configuration code has been saved to: {filePath}" );
            }
            catch( Exception ex )
            {
                Console.WriteLine( $"Unable to save file in {directory}. Error: {ex.Message}" );
            }
        }

        /// <summary>
        /// Exports the default EasyParser configuration to a file named EasyParse.cs.
        /// Although the nested functions in <see cref="ExportDefaultConfig"/> are not conventional, they get the job done for now so i'll just let them be.
        /// </summary>
        public static void ExportDefaultConfig( string? targetDirectory = null, bool exportWithMain = false )
        {
            var configCode = exportWithMain
                ? GetConfigTemplateWithMain()
                : GetConfigTemplateWithoutMain();

            var initialDirectory = GetInitialDirectory();

            var writableDirectory = targetDirectory ?? FindWritableParentDirectory( initialDirectory );
            Console.WriteLine( $"Using directory: {writableDirectory}" );

            SaveConfigFile( writableDirectory, configCode );
        }

        /// <summary>
        ///  Gets the config template string including the public static void Main(string[] args)... part
        /// </summary>
        /// <returns></returns>
        private static string GetConfigTemplateBase()
        {
            return
        @"//AUTO GENERATED USING https://github.com/rohit-kuinkel1/EasyParse/tree/main
using EasyParser.Core;
namespace Program.Parsing
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
    //a -r ""File Name.txt"" -v True --s false --c 10
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

        private static string GetMainMethodTemplate()
        {
            return
@"  
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
    //}";
        }

        private static string GetConfigTemplateWithMain()
        {
            return GetConfigTemplateBase() + "\n" + GetMainMethodTemplate() + "\n}";
        }

        private static string GetConfigTemplateWithoutMain()
        {
            return GetConfigTemplateBase() + "\n}";
        }
    }
}