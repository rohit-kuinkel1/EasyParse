using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyParse
{
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
}
