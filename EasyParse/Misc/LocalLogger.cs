using System;
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
        /// Represents the padding for the log messages.
        /// </summary>
        private static readonly int Padding = 10;

        /// <summary>
        /// Represents the current Time in string format.
        /// </summary>
        internal static readonly string TimeNowString = DateTime.Now.ToString( "MM/dd/yyyy HH:mm:ss.fffff" );

        /// <summary>
        /// default static constructor for <see cref="Logger"/>.
        /// Access specifiers arent allowed for static constructors, hence omitted.
        /// </summary>
        static Logger()
        {
            _minLogLevel = LogLevel.Debug;
        }

        /// <summary>
        /// Initializes the Logger with a specified minimum log level.
        /// </summary>
        /// <param name="minLogLevel">The minimum log level for messages to be logged. Defaults to Debug.</param>
        internal static void Initialize( LogLevel minLogLevel = LogLevel.Debug )
        {
            //if( minLogLevel > LogLevel.BackTrace )
            //{
            _minLogLevel = minLogLevel;
            //}
            //else 
            //{
            //     Logger.Debug( "Cannot set LogLevel.BackTrace for external usage" );
            //_minLogLevel = LogLevel.Debug;
            //}
        }

        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="level">The log level of the message.</param>
        /// <param name="message">The message to log.</param>
        internal static void Log( LogLevel level, string? message )
        {
            // If the log level is lower than the minimum set log level, do not log the message.
            if( level < _minLogLevel )
            {
                return;
            }

            var safeMessage = string.IsNullOrEmpty( message ) ? "" : message;
            var logLevelString = level.ToString().ToUpper().PadRight( Padding );
            var coloredMessage = GetColoredMessage( level, $"[{TimeNowString}] {EasyParseException.Prefix} {logLevelString}: {safeMessage}" );
            Console.WriteLine( coloredMessage );
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
                LogLevel.BackTrace => $"\u001b[38;2;54;69;79m{message}\u001b[0m", // Gray
                LogLevel.Debug => $"\u001b[37m{message}\u001b[0m", // White
                LogLevel.Info => $"\u001b[32m{message}\u001b[0m", // Green
                LogLevel.Warning => $"\u001b[33m{message}\u001b[0m", // Yellow
                LogLevel.Error => $"\u001b[35m{message}\u001b[0m", // Magenta
                LogLevel.Critical => $"\u001b[1;37;41m{message}\u001b[0m", // Bold white text on red background for Critical
                _ => message
            };
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