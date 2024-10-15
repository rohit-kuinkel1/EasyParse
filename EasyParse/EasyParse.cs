using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using EasyParser.Enums;
using EasyParser.Parsing;
using EasyParser.Utility;

namespace EasyParser
{
    /*
     parsing of args and how easyparse will accept args
    EasyParse, for now at least, will focus on pre build command line args
    The args will be parsed in SQL style or human readable format.
    For instance: 
    addFile where Name is TextFile.txt and FilePath is C:/git/Demo and IsSmallerThan is 5KB and IsInFormat is UTF-8 with BOM
    this will be parsed as addFile being the main verb with its options starting from where, meaning Name,FilePath,IsSmallerThan and IsInFormat are options
    with respective values of TextFile.txt, C:/git/Demo, 5KB and UTF-8 with BOM

    for this to work the solution where this library will be used must have this following rough structure.
    [Verb('a', "addFile", Required=false, HelpText="add, Aliases="af")]
    class EasyParse //name can be anything of the class,just this class must have the attribute attached to it
    {
        //option long names are case insensitive, meaning NAME,nAmE or name all are treated the same to avoid nuisance
        [Option('n',"name",Required=true,Description="Name of the file to add", HelpText= "normal file name structure in string format,no need of quotes even for spaces in string")]
        public string FileName{get;set;}

        [Option('p',"FilePath",Required=true,Description="Path where the file is to be added, HelpText= "normal path structure in string format,no need of quotes even for spaces in string")]
        public string FileName{get;set;}
        
        ........

    }
     
    A simple "did you mean" feature will help a lot during parsing.
    One way to achieve this is to use a source generator to already gather the verbs and options during compile time 
    in a ~collection like Dictionary<string,List<string>> where key will be the verb since one verb can have n number of options
    and the value will be a collection of options for that specific verb

    Since we will already have generated this during compile time, runtime lookups for the algorithm we decide to go with will be extremely efficient, almost negligible cost
    and the suggestions will also be almost 100% accurate, making the life of the user easier
     */


    /// <summary>
    /// public entry point for the Parser.
    /// </summary>
    public class EasyParse
    {
        #region privateFields

        /// <summary>
        /// abstraction of the parsing type to parse the args
        /// </summary>
        private IParsing? _parsing;

        /// <summary>
        /// HashSet of reserved keywords during arg parsing.
        /// Contains all the values of the <see cref="EasyParser.Enums.ParsingKeyword"/> in string format.
        /// </summary>
        private static readonly HashSet<string> Keywords = new HashSet<string>( Enum.GetNames( typeof( ParsingKeyword ) ).Select( k => k.ToLowerInvariant() ) );

        #endregion

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
        /// If the natural language syntax was used, 'where' keyword must be used at index 1 to denote that natural language has been used.
        /// If the conventional syntax was used, there is no need to use the 'where' keyword.
        /// Save the value returned by <see cref="Parse{T}(string[])"/> locally to use it.
        /// <code>
        /// //where <typeparamref name="T"/> is your class where you defined the verb and its related options.
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
        /// <returns>Instance of <see cref="ParsingResult{Type}"/> along with <see cref="bool"/> <see cref="ParsingResult{Type}.Success"/> to denote success.</returns>
        /// <exception cref="NullException"> When the provided <paramref name="args"/> was null.</exception>
        /// <exception cref="BadFormatException"> When the provided <paramref name="args"/> was badly formatted.</exception>
        /// <exception cref="IllegalOperation"> When type mismatch occurs or when static/abstract class is provided as type for instance.</exception>
        /// <exception cref="Exception"> For general unforseen exceptions.</exception>
        [Pure]
        public ParsingResult<T> Parse<T>( string[] args ) where T : new()
        {
            try
            {
                _ = Utility.Utility.NotNullValidation( args, true );

                // Determine if we are using Natural Language or Standard Language parsing
                var isStringAtIndex1EqualToWhereKeyword = args.Length > 1 && string.Equals( args[1], ParsingKeyword.Where.ToString(), StringComparison.OrdinalIgnoreCase );
                var containsKeywords = args.Any( arg => Keywords.Contains( arg.ToLowerInvariant() ) );

                if( isStringAtIndex1EqualToWhereKeyword && containsKeywords )
                {
                    _parsing = new NaturalLanguageParsing();
                }
                else if( !containsKeywords )
                {
                    _parsing = new StandardLanguageParsing();
                }
                else
                {
                    throw new BadFormatException( "Invalid structure for input args. Reserved keywords were detected to have been" +
                        "used for standard parsing. Please refrain from mixing natural language and standard language format and try again." );
                }

                // Call the appropriate Parse method and return the result
                return _parsing.Parse<T>( args );
            }
            catch( NullException ex )
            {
                Logger.Critical( $"{ex.GetType()} {ex.Message}" );
                return new ParsingResult<T>( false, ex.Message, default! );
            }
            catch( Exception ex ) // Include StackTrace for general errors including BadFormatException
            {
                Logger.Critical( $"{ex.GetType()} {ex.Message}" );
                Logger.Critical( ex.StackTrace );
                return new ParsingResult<T>( false, ex.Message, default! );
            }
        }

        /// <summary>
        /// Exports the default EasyParser configuration to a file named EasyParse.cs.
        /// Although the nested functions are not conventional, they get the job job so i'll just let them be.
        /// </summary>
        /// <summary>
        /// Exports the default EasyParser configuration to a file named EasyParse.cs.
        /// Although the nested functions are not conventional, they get the job done so I'll just let them be.
        /// </summary>
        public static void ExportDefaultConfig()
        {
            var configCode = @"using EasyParser.Core;
namespace MyParser
{
    /// <summary>
    /// <para>
    /// The demo <see cref=""ParseVerbs""/> class encapsulates the command-line options for the EasyParse library. 
    /// The class name does not necessarily always have to be <see cref=""ParseVerbs""/>; it is up to you to decide the name.
    /// Whatever the name, you need to provide it as a generic argument to <see cref=""EasyParser.EasyParse.Parse{T}(string[])""/>.
    /// <see cref=""ParseVerbs""/> is used in conjunction with the <see cref=""VerbAttribute""/> to define verbs and their associated options.
    /// </para>
    /// <para>
    /// <see cref=""MutualAttribute""/> denotes the mutual relationship between two <see cref=""OptionsAttribute""/>.
    /// The enums <see cref=""EasyParser.Enums.MutualType""/> can be used to denote a 
    /// <see cref=""EasyParser.Enums.MutualType.Exclusive""/> or 
    /// <see cref=""EasyParser.Enums.MutualType.Inclusive""/> relationship between two <see cref=""OptionsAttribute""/>.
    /// If a property is marked to be mutually inclusive to another property, then the mutual relationship takes precedence over the <see cref=""OptionsAttribute.Required""/>
    /// property, meaning even if an attribute is set to <see cref=""OptionsAttribute.Required""/> = <see langword=""false""/> but is mentioned as a mutually inclusive attribute
    /// to another attribute, the <see cref=""OptionsAttribute.Required""/> for that particular option will still be interpreted as being set to 
    /// <see cref=""OptionsAttribute.Required""/> = <see langword=""true""/>.
    /// This is particularly useful for instances where 2 entities aren't necessarily required, but if they are, they are required together.
    /// </para>
    /// </summary>
    [Verb( 'a', ""add"", Required = false, HelpText = ""Add file contents to the index."" )]
    public class ParseVerbs
    {
        [Options( 'r', ""read"", Default = null, Required = false, HelpText = ""Input files to be processed."", Aliases = new[] { ""reading"", ""studying"" } )]
        [Mutual( EasyParser.Enums.MutualType.Inclusive, nameof( Count ), nameof( Stdin ) )]
        public string? InputFile { get; set; }

        [Options( 'v', ""verbose"", Default = false, Required = true, HelpText = ""Prints all messages to standard output."" )]
        public bool Verbose { get; set; }

        [Options( 's', ""stdin"", Default = false, Required = false, HelpText = ""Read from stdin"", Aliases = new[] { ""standardin"", ""stdinput"" } )]
        public bool Stdin { get; set; }

        [Options( 'c', ""count"", Default = 0, Required = false, HelpText = ""Count of verbs"", Aliases = new[] { ""length"", ""total"" } )] //Len of Aliases >=2 , else they are ignored
        public int Count { get; set; }
    }
    
    /// <summary>
    /// Entry Point of an application.
    /// </summary>
    /// <param name=""args""></param>
    //public static void Main( string[] args )
    //{
        //LogLevel.BackTrace cannot be set by the users; levels => Debug, Info, Warning, Error, Critical, None
        //var parser = new EasyParse( LogLevel.Debug, false );
        //var parsingResult = parser.Parse<ParseVerbs>( args );
        //if( parsingResult.Success )
        //{
            // do something
        //}
        //else
        //{
            // do something
            // Console.WriteLine( parsingResult.ErrorMessage );
        //}
    //}
}";

            string SaveConfigFile( string directory, string fileName, string content )
            {
                try
                {
                    var filePath = Path.Combine( directory, fileName );
                    File.WriteAllText( filePath, content );
                    return filePath;
                }
                catch( Exception ex )
                {
                    Console.WriteLine( $"Unable to save file in {directory}. Error: {ex.Message}" );
                    return null;
                }
            }

            string GetWritableDirectory()
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if( entryAssembly == null )
                {
                    Console.WriteLine( "Unable to determine the entry assembly." );
                    return Directory.GetCurrentDirectory();
                }

                var currentDirectory = Path.GetDirectoryName( entryAssembly.Location );
                if( currentDirectory == null )
                {
                    Console.WriteLine( "Unable to determine the current directory." );
                    return Directory.GetCurrentDirectory();
                }

                for( int i = 0; i < 3; i++ )
                {
                    try
                    {
                        var parentDirectory = Directory.GetParent( currentDirectory )?.FullName;
                        if( parentDirectory == null )
                        {
                            Console.WriteLine( $"Reached the root directory. Using current directory: {currentDirectory}" );
                            break;
                        }

                        // Test if we can write to this directory
                        var testFile = Path.Combine( parentDirectory, "test_write_permission.tmp" );
                        File.WriteAllText( testFile, "test" );
                        File.Delete( testFile );

                        currentDirectory = parentDirectory;
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( $"Cannot access or write to parent directory. Using current directory: {currentDirectory}. Error: {ex.Message}" );
                        break;
                    }
                }

                return currentDirectory;
            }

            var projectDirectory = GetWritableDirectory();
            Console.WriteLine( $"Using directory: {projectDirectory}" );

            var savedFilePath = SaveConfigFile( projectDirectory, "EasyParseOptions.cs", configCode );

            if( savedFilePath != null )
            {
                Console.WriteLine( $"Configuration code has been saved to: {savedFilePath}" );
            }
            else
            {
                Console.WriteLine( "Failed to save the configuration file." );
            }
        }

    }
}
