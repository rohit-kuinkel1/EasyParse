using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using EasyParse.Misc;
using EasyParse.Core;
using EasyParse.Enums;
using EasyParse.Parsing;
using EasyParse.Utility;

namespace EasyParse
{
    /// <summary>
    /// public entry point for the Parser.
    /// </summary>
    public sealed partial class EasyParser
    {
        /// <summary>
        /// abstraction of the parsing type to parse the args
        /// </summary>
        private IParsing? _parsing;

        /// <summary>
        /// HashSet of reserved keywords during arg parsing.
        /// Contains all the values of the <see cref="EasyParse.Enums.ParsingKeyword"/> in string format.
        /// </summary>
        private static readonly HashSet<string> Keywords = new HashSet<string>(
            Enum.GetNames( typeof( ParsingKeyword ) )
                .Select( k => k.ToLowerInvariant() )
        );

        /// <summary>
        /// Parameterized Constructor for <see cref="EasyParser"/>.
        /// Set the <paramref name="minLogLevel"/> to the desired minimum logLevel from <see cref="LogLevel"/>.
        /// Set the <paramref name="redirectLogsToFile"/> to <see langword="true"/> if you want to redirect all the logs to the desired directory.
        /// Set the <paramref name="logDirectory"/> to the desired directory to redirect file logs to. When <paramref name="logDirectory"/> is set to <see langword="null"/>
        /// or default is used, then the <see cref="Logger"/> class will internally set <paramref name="logDirectory"/> to <c>AppDomain.CurrentDomain.BaseDirectory</c>
        /// </summary>
        /// <remarks>
        /// <see cref="LogLevel.BackTrace"/> cannot be set by users.
        /// </remarks>
        private EasyParser( 
            LogLevel minLogLevel = LogLevel.Info, 
            bool redirectLogsToFile = false, 
            string? logDirectory = null 
        )
        {
            Logger.Initialize( minLogLevel, redirectLogsToFile, logDirectory );
        }

        #region Singelton
        /// <summary>
        /// <see cref="_instance"/> holds the singelton instance for <see cref="EasyParser"/>
        /// </summary>
        private static EasyParser? _instance;

        /// <summary>
        /// public constructor call for <see cref="EasyParser"/>
        /// </summary>
        /// <param name="minLogLevel"></param>
        /// <param name="redirectLogsToFile"></param>
        /// <param name="logDirectory"></param>
        /// <returns></returns>
        // Helps keep the code base for the users of the tool cleaner since they dont have to hassle with instantiating an instance with new
        public static EasyParser Initialize(
            LogLevel minLogLevel = LogLevel.Info,
            bool redirectLogsToFile = false,
            string? logDirectory = null
        )
        {
            if( _instance == null )
            {
                _instance = new EasyParser( minLogLevel, redirectLogsToFile, logDirectory );
            }
            return _instance;
        }

        /// <summary>
        /// Auto GET property to access the singelton instance of <see cref="EasyParser"/>
        /// </summary>
        public static EasyParser Instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = new EasyParser();
                }

                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// <see cref="SetLoggerStatusEnabled(bool)"/> sets the logger on or off depending on whether <see langword="true"/> or <see langword="false"/>
        /// was passed to it. 
        /// </summary>
        /// <remarks>
        /// Keep in mind however, when <see cref="Logger"/> is disabled (for example by passing <see langword="false"/> to <see cref="SetLoggerStatusEnabled(bool)"/>),
        /// the internal logs from <see cref="EasyParser"/> cannot be viewed or exported.
        /// Alternatively you can pass <see cref="LogLevel.None"/> to <see cref="EasyParser"/> constructor 
        /// which will do the same as passing <see langword="false"/> to <see cref="SetLoggerStatusEnabled(bool)"/>
        /// </remarks>
        /// <param name="isLoggerEnabled"></param>
        public void SetLoggerStatusEnabled( bool isLoggerEnabled )
        {
            Logger.IsLoggerEnabled = isLoggerEnabled;
        }

        /// <summary>
        /// Parses the arguments provided to an instance of <see cref="EasyParser"/> and delegates the processing to the respective class.
        /// <para>
        /// If the natural language syntax was used, the keyword 'where' must be used at index 1 to denote that natural language has been used.
        /// Example: add where FilePath is ABCDEFG/HIJKLMNOP Contains is "photos, text, data" IsPasswordLocked is false
        /// </para>
        /// <para>
        /// If the conventional syntax was used, there is no need to use the 'where' keyword.
        /// Example: add --FilePath ABCDEFG/HIJKLMNOP --Contains "photos, text, data" --IsPasswordLocked false
        /// </para>
        /// Save the value returned by <see cref="ParseOne{T}(string[])"/> locally to use it.
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
        /// <exception cref="IllegalOperationException"> When type mismatch occurs or when static/abstract class is provided as type for instance.</exception>
        /// <exception cref="Exception"> For general unforseen exceptions.</exception>
        public ParsingResult<T> ParseOne<T>( string[] args ) where T : class, new()
        {
            try
            {
                _ = Utility.Utility.NotNullValidation( args, throwIfNull: true );

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
                        "Invalid structure for input args. Please refer to the documentation."
                    );
                }

                return _parsing.ParseOne<T>( args );
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
        [SupportedOSPlatform( "windows" )]
        [SupportedOSPlatform( "linux" )]
        public static void ExportDefaultConfig( 
            string? targetDirectory = null, 
            bool exportWithMain = false 
        )
        {
            var configCode = exportWithMain
                ? Template.GetConfigTemplateWithMain()
                : Template.GetConfigTemplateWithoutMain();

            var initialDirectory = FileHandler.GetInitialDirectory();

            var writableDirectory = targetDirectory ?? FileHandler.FindWritableParentDirectory( initialDirectory );

            Logger.Info( $"Using directory: {writableDirectory}" );
            FileHandler.SaveConfigFile( writableDirectory, configCode );
        }
    }
}