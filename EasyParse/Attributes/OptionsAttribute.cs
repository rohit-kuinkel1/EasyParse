using System;

namespace EasyParse.Core
{
    /// <summary>
    /// Attribute for defining command-line options.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    internal sealed class OptionsAttribute : BaseAttribute
    {
        /// <summary>
        /// Gets the short name for the option (single-character).
        /// </summary>
        public char ShortName { get; }

        /// <summary>
        /// Gets or sets the default value for the option.
        /// </summary>
        public object? DefaultValue { get; set; }

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
        public OptionsAttribute(
            char shortName,
            string longName,
            bool isRequired = false,
            object? defaultValue = null,
            string helpText = "",
            string errorMessage = ""
        ) 
            : base( longName, isRequired, helpText )
        {
            ShortName = shortName;
            DefaultValue = defaultValue;
            ErrorMessage = errorMessage;
        }
    }
}
