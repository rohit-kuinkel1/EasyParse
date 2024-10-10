using System;

namespace EasyParse.Core
{
    /// <summary>
    /// Attribute for defining command-line options.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    public sealed class OptionsAttribute : BaseAttribute
    {
        /// <summary>
        /// Gets the short name for the option (single-character).
        /// </summary>
        public char ShortName { get; }

        /// <summary>
        /// Gets the long name given to this option.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Specifies the necessity of the attribute.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets the default value for the option.
        /// </summary>
        public object? Default { get; set; }

        /// <summary>
        /// Gets or sets the custom error message for invalid input.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name for the option (single character).</param>
        /// <param name="longName">The long name for the option.</param>
        /// <param name="isRequired">Indicates whether the option is required.</param>
        /// <param name="defaultValue">The default value for the option.</param>
        /// <param name="helpText">The help text for the option.</param>
        /// <param name="errorMessage">The custom error message for invalid input.</param>
        /// <param name="aliases">The aliases for the verb.</param>
        public OptionsAttribute(
            char shortName,
            string longName,
            bool isRequired = false,
            object? defaultValue = null,
            string helpText = "",
            string errorMessage = "",
            params string[] aliases
        )
            : base( helpText, aliases )
        {
            ShortName = shortName;
            LongName = longName;
            Required = isRequired;
            Default = defaultValue;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="OptionsAttribute"/> instance,
        /// including its short name, long name, whether it is required, default value, help text, and error message.
        /// </summary>
        /// <returns>A string representing the options attribute.</returns>
        public override string ToString()
        {
            return $"OptionsAttribute: {LongName} (Short Name: {ShortName}, " +
                   $"Is Required: {Required}, " +
                   $"Default Value: {Default}, " +
                   $"Help Text: {HelpText}, " +
                   $"Error Message: {ErrorMessage}) \n";
        }
    }
}
