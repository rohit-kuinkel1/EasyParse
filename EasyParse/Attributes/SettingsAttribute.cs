using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyParser.Core
{
    /// <summary>
    /// Attribute for defining settings for options.
    /// Includes properties for color, validation ranges, and regex pattern matching.
    /// This class cannot be inherited.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    public sealed class SettingsAttribute : BaseAttribute
    {
        #region FieldProperties
        /// <summary>
        /// placeholder for default values for MinValue and MaxValue
        /// </summary>
        internal const int DefaultNotProvidedMinMax = -999999999;

        /// <summary>
        /// holsd the validated minimum value possible for this <see cref="OptionsAttribute"/>
        /// </summary>
        [Validated] private int minValue = DefaultNotProvidedMinMax;

        /// <summary>
        /// holds the validated maximum value possible for this <see cref="OptionsAttribute"/>
        /// </summary>
        [Validated] private int maxValue = DefaultNotProvidedMinMax;

        /// <summary>
        /// holds the validated supplied regex pattern to be used for pattern matching during parsing
        /// </summary>
        [Validated] private string regexPattern = string.Empty;

        /// <summary>
        /// holds the error message to be shown explicitly only when the regex validation fails.
        /// </summary>
        [Validated] private string regexOnFailureMessage = "Validation against Regex Pattern failed";
        #endregion

        #region AutoProperties
        /// <summary>
        /// Minimum value for validation (used for numeric types).
        /// When used in conjuction with <see cref="OptionsAttribute"/> it helps 
        /// set a lower limit to the class property. 
        /// </summary>
        /// <remarks>
        /// The Class needs to have <see cref="VerbAttribute"/> as its decorator for <see cref="OptionsAttribute"/> and <see cref="SettingsAttribute"/> to work on the classes properties
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int MinValue
        {
            get => minValue;
            set
            {
                if( maxValue != DefaultNotProvidedMinMax && value > maxValue )
                {
                    throw new ArgumentOutOfRangeException( $"{nameof( MinValue )} cannot be bigger than {nameof( MaxValue )}" );
                }

                minValue = value;
            }
        }

        /// <summary>
        /// Maximum value for validation (used for numeric types).
        /// When used in conjuction with <see cref="OptionsAttribute"/> it helps 
        /// set an upper limit to the class property.
        /// </summary>
        /// <remarks>
        /// The Class needs to have <see cref="VerbAttribute"/> as its decorator for <see cref="OptionsAttribute"/> and <see cref="SettingsAttribute"/> to work on the classes properties
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int MaxValue
        {
            get => maxValue;
            set
            {
                if( minValue != DefaultNotProvidedMinMax && value < minValue )
                {
                    throw new ArgumentOutOfRangeException( $"{nameof( MaxValue )} cannot be less than {nameof( MinValue )}" );
                }

                maxValue = value;
            }
        }

        /// <summary>
        /// Array of allowed values that can be passed by the user.
        /// When specified, only these values will be considered valid for the option.
        /// </summary>
        [NoValidationRequired] public object[]? AllowedValues { get; set; }

        /// <summary>
        /// Defines the regex pattern to be used to be set for <see cref="CompiledRegex"/>
        /// If the supplied value was null or empty or just whitespaces, then Regex validation will obviously be done because 
        /// the <see cref="CompiledRegex"/> will remain <see langword="null"/>
        /// </summary>
        public string RegexPattern
        {
            get => regexPattern;
            set
            {
                regexPattern = value;

                if( !string.IsNullOrEmpty( value ) && !string.IsNullOrWhiteSpace( value ) )
                {
                    CompiledRegex = new Regex(
                        value,
                        RegexOptions.Compiled,
                        TimeSpan.FromMilliseconds( 500 )
                    );
                }
                else
                {
                    Logger.Error( $"Did not create a Regex instance out of {nameof( value )}:{value} because its invalid." +
                        $"{nameof( CompiledRegex )} for {nameof( SettingsAttribute )} is still set to 'NULL'" );
                }
            }
        }

        /// <summary>
        /// Compiled Regex pattern to validate the option's value.
        /// When set on a class property (class needs to have <see cref="VerbAttribute"/> as its decorator) as a
        /// helping attribute to the <see cref="OptionsAttribute"/> it can 
        /// be used to leverage runtime regex pattern matching checks automatically.
        /// </summary>
        internal Regex? CompiledRegex { get; private set; } = null;

        /// <summary>
        /// Error message for regex validation.
        /// </summary>
        public string RegexOnFailureMessage
        {
            get => regexOnFailureMessage;
            set => regexOnFailureMessage = !string.IsNullOrWhiteSpace( value )
                ? value
                : "Validation against the provided Regex Pattern failed";
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsAttribute"/> class.
        /// </summary>
        /// <param name="minValue">The minimum value for validation.</param>
        /// <param name="maxValue">The maximum value for validation.</param>
        /// <param name="helpText"> The help text to show for <see cref="SettingsAttribute"/></param>
        /// <param name="regexPattern">Regex pattern for value validation.</param>
        /// <param name="regexErrorMessage">Error message for regex validation.</param>
        /// <param name="allowedValues">Represents all the allowed values that can be passed as value for the property that has this decorator</param>
        public SettingsAttribute(
            int minValue = DefaultNotProvidedMinMax,
            int maxValue = DefaultNotProvidedMinMax,
            string helpText = "",
            string regexPattern = "",
            string regexErrorMessage = "",
            object[]? allowedValues = null
        )
            : base( string.Empty, helpText, Array.Empty<string>() ) //array.empty<T>() is slightly better than [] in terms of performance
        {
            MinValue = minValue;
            MaxValue = maxValue;
            RegexOnFailureMessage = regexErrorMessage;
            RegexPattern = regexPattern;
            AllowedValues = allowedValues?.ToArray();
        }
        #endregion

        #region Misc
        /// <summary>
        /// Returns a string representation of the <see cref="SettingsAttribute"/> instance,
        /// including its properties.
        /// </summary>
        /// <returns>A string representing the options attribute.</returns>
        public override string ToString()
        {
            return
                $"\n\t\t{nameof(SettingsAttribute)}: \n" +
                $"\t\t\t{nameof( MinValue )}:{MinValue}, \n" +
                $"\t\t\t{nameof( MaxValue )}:{MaxValue}, \n" +
                $"\t\t\t{nameof( RegexPattern )}:{RegexPattern}, \n" +
                $"\t\t\t{nameof( RegexOnFailureMessage )}:{RegexOnFailureMessage}, \n";
        }
        #endregion
    }
}
