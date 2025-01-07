using System;
using System.IO;
using EasyParser.Utility;

namespace EasyParser
{
    #region enum LogLevel
    /// <summary>
    /// LogLevels for our internal logger.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// For internal use case only. 
        /// Set to <see cref="BackTrace"/> if you wish to see detailed internal logs from EasyParse.
        /// </summary>
        BackTrace,

        /// <summary>
        /// Log level for debugging purposes.
        /// Set to <see cref="Debug"/> if you wish to see debug logs from EasyParse.
        /// </summary>
        Debug,

        /// <summary>
        /// Log level for general informational messages.
        /// Set to <see cref="Info"/> if you wish to see informational logs from EasyParse.
        /// </summary>
        Info,

        /// <summary>
        /// Log level for potential issues that may require attention.
        /// Set to <see cref="Warning"/> if you wish to see warning logs from EasyParse.
        /// </summary>
        Warning,

        /// <summary>
        /// Log level for errors that have occurred.
        /// Set to <see cref="Error"/> if you wish to see error logs from EasyParse.
        /// </summary>
        Error,

        /// <summary>
        /// Log level for critical errors that require immediate attention.
        /// Set to <see cref="Critical"/> if you wish to see critical logs from EasyParse.
        /// </summary>
        Critical,

        /// <summary>
        /// Set to <see cref="None"/> if you wish to see no logging from EasyParse.
        /// </summary>
        None,
    }
    #endregion

    /// <summary>
    /// Internal logger for <see cref="EasyParse"/>
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// minimum LogLevel for <see cref="EasyParse"/>.
        /// Only log messages equal or above this level will be printed to the console.
        /// </summary>
        private static LogLevel _minLogLevel;

        /// <summary>
        /// minimum LogLevel for <see cref="EasyParse"/>.
        /// Only log messages equal or above this level will be printed to the console.
        /// </summary>
        public static LogLevel MinLogLevel
        {
            get => _minLogLevel;
            set => _minLogLevel = value;
        }

        /// <summary>
        /// Represents the padding for the log messages.
        /// </summary>
        private static readonly int Padding = 10;

        /// <summary>
        /// Represents the current Time in string format.
        /// </summary>
        internal static readonly string TimeNowString = DateTime.Now.ToString( "MM/dd/yyyy HH:mm:ss.fffff" );

        /// <summary>
        /// Flag to indicate whether to redirect all the logs to a logFile.
        /// </summary>
        internal static bool RedirectLogsToFile { get; set; } = false;

        /// <summary>
        /// Final subdirectory for the log dump
        /// </summary>
        internal const string EasyParseLogDir = "EasyParserLogs";

        /// <summary>
        /// Represents the base directory where a new directory <see cref="EasyParseLogDir"/> will be created 
        /// which will hold all the logs from <see cref="EasyParser"/>
        /// </summary>
        private static string BaseLogDirectory { get; set; }

        /// <summary>
        /// Represents the final directory where the log files will reside.
        /// Is a combination of <see cref="BaseLogDirectory"/> and <see cref="EasyParseLogDir"/> at the end.
        /// </summary>
        private static string FinalLogDirectory { get; set; }

        /// <summary>
        /// Flag to enable or disable logging. When set to <see langword="false"/>, no logs will be written.
        /// </summary>
        private static bool _isLoggerEnabled = true;

        /// <summary>
        /// Gets or sets whether logging is enabled.
        /// </summary>
        public static bool IsLoggerEnabled
        {
            get => _isLoggerEnabled;
            set => _isLoggerEnabled = value;
        }


        /// <summary>
        /// default static constructor for <see cref="Logger"/>.
        /// Access specifiers arent allowed for static constructors, hence omitted.
        /// </summary>
        static Logger()
        {
            _minLogLevel = LogLevel.Debug;
            BaseLogDirectory = AppDomain.CurrentDomain.BaseDirectory;
            FinalLogDirectory = string.Concat( BaseLogDirectory, EasyParseLogDir );
        }

        /// <summary>
        /// Resets the Logger to its initial state.
        /// </summary>
        internal static void Reset()
        {
            _minLogLevel = LogLevel.Debug;
            _isLoggerEnabled = false;
            RedirectLogsToFile = false;
            BaseLogDirectory = AppDomain.CurrentDomain.BaseDirectory;
            FinalLogDirectory = string.Concat( BaseLogDirectory, EasyParseLogDir );
        }

        /// <summary>
        /// Initializes the Logger with a specified minimum log level.
        /// </summary>
        /// <param name="minLogLevel">The minimum log level for messages to be logged. Defaults to Debug.</param>
        /// <param name="redirectLogsToFile"> Flag to specify the redirection of all the logs to a file.</param>
        /// <param name="baseLogDirectory"> The base directory passed by the user for the log files to be stored in.</param>
        internal static void Initialize( LogLevel minLogLevel = LogLevel.Debug, bool redirectLogsToFile = false, string? baseLogDirectory = null )
        {
            RedirectLogsToFile = redirectLogsToFile;
            FinalLogDirectory = !string.IsNullOrEmpty( baseLogDirectory )
                ? Path.Combine( baseLogDirectory, EasyParseLogDir )
                : Path.Combine( BaseLogDirectory, EasyParseLogDir );

            CreateDirectoryIfNotExists( FinalLogDirectory );

            if( Logger.IsLoggerEnabled && !redirectLogsToFile )
            {
                Logger.Debug( $"Set {nameof( BaseLogDirectory )} to point to {BaseLogDirectory}" );
            }

            //if( minLogLevel > LogLevel.BackTrace )
            //{
            _minLogLevel = minLogLevel;
            //}
            //else
            //{
            //    Logger.Debug( $"Cannot set LogLevel.BackTrace for external usage, setting it to {LogLevel.Debug} instead" );
            //    _minLogLevel = LogLevel.Debug;
            //}
        }

        private static void CreateDirectoryIfNotExists( string? directory = null )
        {
            var dir = directory ?? FinalLogDirectory;

            if( !Directory.Exists( dir ) )
            {
                Directory.CreateDirectory( dir );
            }
        }

        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="level">The log level of the message.</param>
        /// <param name="message">The message to log.</param>
        internal static void Log( LogLevel level, string? message )
        {
            if( !_isLoggerEnabled || level < _minLogLevel )
            {
                return;
            }

            var safeMessage = string.IsNullOrEmpty( message ) ? "" : message;
            var logLevelString = level.ToString().ToUpper().PadRight( Padding );
            var formattedMessage = $"[{TimeNowString}] {EasyParseException.Prefix} {logLevelString}: {safeMessage}";
            var coloredMessage = GetColoredMessage( level, formattedMessage );

            if( RedirectLogsToFile )
            {
                WriteLogToFile( formattedMessage );
            }
            else
            {
                Console.WriteLine( coloredMessage );
            }
        }


        /// <summary>
        /// Gets a colored message based on the log level.
        /// </summary>
        /// <param name="level">The log level associated with the message.</param>
        /// <param name="message">The message to color.</param>
        /// <returns>A colored string representation of the message.</returns>
        private static string GetColoredMessage( LogLevel level, string message )
        {
            return level switch
            {
                LogLevel.BackTrace => $"\u001b[38;2;54;69;79m{message}\u001b[0m", //gray
                LogLevel.Debug => $"\u001b[37m{message}\u001b[0m", //white
                LogLevel.Info => $"\u001b[32m{message}\u001b[0m", //green
                LogLevel.Warning => $"\u001b[33m{message}\u001b[0m", //yellow
                LogLevel.Error => $"\u001b[35m{message}\u001b[0m", //magenta
                LogLevel.Critical => $"\u001b[1;37;41m{message}\u001b[0m", //bold white text on red background for Critical
                _ => message
            };
        }

        /// <summary>
        /// Writes a log message to the log file.
        /// </summary>
        /// <param name="message">The message to log in the file.</param>
        private static void WriteLogToFile( string message )
        {
            try
            {
                //final check, if the dir doesnt exist, create it
                CreateDirectoryIfNotExists( FinalLogDirectory );

                var logFilePath = Path.Combine( FinalLogDirectory, $"EasyParser_{DateTime.Now:yyyy_MM_dd_HH_mm}.log" );
                File.AppendAllText( logFilePath, message + Environment.NewLine );
            }
            catch( Exception ex ) //in this case log all the exceptions
            {
                Logger.Critical( $"Failed to write to the specified log file: {ex.Message}" );
            }
        }



        /// <summary>
        /// Logs a BackTrace level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void BackTrace( string? message ) => Log( LogLevel.BackTrace, message );

        /// <summary>
        /// Logs a Debug level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void Debug( string? message ) => Log( LogLevel.Debug, message );

        /// <summary>
        /// Logs an Info level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void Info( string? message ) => Log( LogLevel.Info, message );

        /// <summary>
        /// Logs a Warning level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void Warn( string? message ) => Log( LogLevel.Warning, message );

        /// <summary>
        /// Logs an Error level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void Error( string? message ) => Log( LogLevel.Error, message );

        /// <summary>
        /// Logs a Critical level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void Critical( string? message ) => Log( LogLevel.Critical, message );
    }
}