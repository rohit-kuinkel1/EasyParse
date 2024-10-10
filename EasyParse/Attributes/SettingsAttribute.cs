using System;
using System.Text.RegularExpressions;

namespace EasyParse.Core
{
    /// <summary>
    /// Attribute for defining settings for options.
    /// Includes properties for color, validation ranges, and regex pattern matching.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    internal sealed class SettingsAttribute : BaseAttribute
    {
        /// <summary>
        /// The color associated with the option (e.g., for console output).
        /// </summary>
        public ConsoleColor? Color { get; }

        /// <summary>
        /// Minimum value for validation (used for numeric types).
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// Maximum value for validation (used for numeric types).
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// Compiled Regex pattern to validate the option's value.
        /// </summary>
        public Regex? CompiledRegex { get; }

        /// <summary>
        /// Error message for regex validation.
        /// </summary>
        public string? RegexErrorMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsAttribute"/> class.
        /// </summary>
        /// <param name="color">The color associated with the option.</param>
        /// <param name="minValue">The minimum value for validation.</param>
        /// <param name="maxValue">The maximum value for validation.</param>
        /// <param name="regexPattern">Regex pattern for value validation.</param>
        /// <param name="regexErrorMessage">Error message for regex validation.</param>
        public SettingsAttribute(
            ConsoleColor? color = null,
            int? minValue = null,
            int? maxValue = null,
            string regexPattern = "",
            string regexErrorMessage = ""
        )
            : base( string.Empty, Array.Empty<string>() ) //array.empty<T>() is slightly better than [] in terms of performance
        {
            Color = color;
            MinValue = minValue;
            MaxValue = maxValue;
            RegexErrorMessage = regexErrorMessage;

            if( !string.IsNullOrWhiteSpace( regexPattern ) )
            {
                CompiledRegex = new Regex( regexPattern, RegexOptions.Compiled );
            }
        }
    }
}
