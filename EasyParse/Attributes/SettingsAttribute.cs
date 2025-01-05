using System;
using System.Text.RegularExpressions;

namespace EasyParser.Core
{
    #region UnderConstruction
    /// <summary>
    /// Attribute for defining settings for options.
    /// Includes properties for color, validation ranges, and regex pattern matching.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    public sealed class SettingsAttribute : BaseAttribute
    {
        /// <summary>
        /// placeholder for default values for MinValue and MaxValue
        /// </summary>
        internal const int DefaultNotProvidedMinMax = -999;

        /// <summary>
        /// Minimum value for validation (used for numeric types).
        /// When used in conjuction with <see cref="OptionsAttribute"/> it helps 
        /// set a lower limit to the class property. 
        /// (Class needs to have <see cref="VerbAttribute"/> as its decorator)
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Maximum value for validation (used for numeric types).
        /// When used in conjuction with <see cref="OptionsAttribute"/> it helps 
        /// set an upper limit to the class property.
        /// (class needs to have <see cref="VerbAttribute"/> as its decorator)
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Array of allowed values that can be passed by the user.
        /// When specified, only these values will be considered valid for the option.
        /// </summary>
        public object[]? AllowedValues { get; set; }

        /// <summary>
        /// Defines the regex pattern to be used to be set for <see cref="CompiledRegex"/>
        /// </summary>
        public string RegexPattern { get; set; }

        /// <summary>
        /// Compiled Regex pattern to validate the option's value.
        /// When set on a class property (class needs to have <see cref="VerbAttribute"/> as its decorator) as a
        /// helping attribute to the <see cref="OptionsAttribute"/> it can 
        /// be used to leverage runtime regex pattern matching checks automatically.
        /// </summary>
        internal Regex? CompiledRegex { get; set; }

        /// <summary>
        /// Error message for regex validation.
        /// </summary>
        public string? RegexOnFailureMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsAttribute"/> class.
        /// </summary>
        /// <param name="minValue">The minimum value for validation.</param>
        /// <param name="maxValue">The maximum value for validation.</param>
        /// <param name="regexPattern">Regex pattern for value validation.</param>
        /// <param name="regexErrorMessage">Error message for regex validation.</param>
        /// <param name="allowedValues"> Represents all the allowed values that can be passed as value for the property that has this decorator</param>
        public SettingsAttribute(
            int minValue = DefaultNotProvidedMinMax,
            int maxValue = DefaultNotProvidedMinMax,
            string regexPattern = "",
            string regexErrorMessage = "",
            object[]? allowedValues = null
        )
            : base( string.Empty, Array.Empty<string>() ) //array.empty<T>() is slightly better than [] in terms of performance
        {
            MinValue = minValue;
            MaxValue = maxValue;
            RegexOnFailureMessage = regexErrorMessage;
            RegexPattern = regexPattern;
            AllowedValues = allowedValues;

            if( !string.IsNullOrWhiteSpace( regexPattern ) )
            {
                CompiledRegex = new Regex(
                    regexPattern,
                    RegexOptions.Compiled,
                    TimeSpan.FromMilliseconds( 500 ) );
            }
        }
    }
    #endregion
}
