using System;

namespace EasyParser
{
    /// <summary>
    /// LogLevels for our internal logger.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Log level debug for debugging.
        /// </summary>
        Debug,
        /// <summary>
        /// Log level info for information.
        /// </summary>
        Info,
        /// <summary>
        /// Log warnings.
        /// </summary>
        Warning,
        /// <summary>
        /// Log errors.
        /// </summary>
        Error,
        /// <summary>
        /// Log fatal errors.
        /// </summary>
        Critical
    }

    internal static class Logger
    {


        private static LogLevel _minLogLevel;

        static Logger()
        {
            _minLogLevel = LogLevel.Debug;
        }

        internal static void Initialize( LogLevel minLogLevel = LogLevel.Debug )
        {
            _minLogLevel = minLogLevel;
        }

        internal static void SetMinLogLevel( LogLevel level )
        {
            _minLogLevel = level;
        }

        internal static void Log( LogLevel level, string message )
        {
            if( level < _minLogLevel )
            {
                return;
            }
            var timestamp = DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" );
            var logLevelString = level.ToString().ToUpper().PadRight( 11 );
            var coloredMessage = GetColoredMessage( level, $"[{timestamp}] {logLevelString}: {message}" );
            Console.WriteLine( coloredMessage );
        }

        private static string GetColoredMessage( LogLevel level, string message )
        {
            return level switch
            {
                LogLevel.Debug => $"\u001b[37m{message}\u001b[0m", // White
                LogLevel.Information => $"\u001b[32m{message}\u001b[0m", // Green
                LogLevel.Warning => $"\u001b[33m{message}\u001b[0m", // Yellow
                LogLevel.Error => $"\u001b[31m{message}\u001b[0m", // Red
                LogLevel.Critical => $"\u001b[35m{message}\u001b[0m", // Magenta
                _ => message
            };
        }

        internal static void Debug( string message ) => Log( LogLevel.Debug, message );
        internal static void Info( string message ) => Log( LogLevel.Information, message );
        internal static void Warn( string message ) => Log( LogLevel.Warning, message );
        internal static void Error( string message ) => Log( LogLevel.Error, message );
        internal static void Critical( string message ) => Log( LogLevel.Critical, message );
    }
}