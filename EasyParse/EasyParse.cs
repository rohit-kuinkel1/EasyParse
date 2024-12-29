using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.Misc;
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

        /// <summary>
        /// Exports the default EasyParser configuration to a file named EasyParse.cs.
        /// Although the nested functions in <see cref="ExportDefaultConfig"/> are not conventional, they get the job done for now so i'll just let them be.
        /// </summary>
        public static void ExportDefaultConfig( string? targetDirectory = null, bool exportWithMain = false )
        {
            var configCode = exportWithMain
                ? GetConfigTemplateWithMain()
                : GetConfigTemplateWithoutMain();

            var initialDirectory = FileHandler.GetInitialDirectory();

            var writableDirectory = targetDirectory ?? FileHandler.FindWritableParentDirectory( initialDirectory );
            Console.WriteLine( $"Using directory: {writableDirectory}" );

            FileHandler.SaveConfigFile( writableDirectory, configCode );
        }

        private static string GetConfigTemplateWithMain()
        {
            return Template.GetConfigTemplateBase() + "\n" + Template.GetMainMethodTemplate() + "\n}";
        }

        private static string GetConfigTemplateWithoutMain()
        {
            return Template.GetConfigTemplateBase() + "\n}";
        }
    }
}