using System;

namespace Parser
{
    /// <summary>
    /// Attribute for defining command-line options.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    public class OptionsAttribute : Attribute
    {
        /// <summary>
        /// Gets the short name for the option (single-character).
        /// </summary>
        public char ShortName { get; }

        /// <summary>
        /// Gets the long name for the option.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Indicates whether the option is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Optional
        /// Gets or sets the default value for the option.
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the help text for the option.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets the custom error message for invalid input.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name for the option (single character).</param>
        /// <param name="longName">The long name for the option.</param>
        /// <param name="required">Indicates whether the option is required.</param>
        /// <param name="defaultValue">The default value for the option.</param>
        /// <param name="helpText">The help text for the option.</param>
        /// <param name="errorMessage">The custom error message for invalid input.</param>
        public OptionsAttribute( 
            char shortName,
            string longName, 
            bool required = false, 
            object? defaultValue = null,
            string helpText = "",
            string errorMessage = "" )
        {
            ShortName = shortName;                 // char
            LongName = longName;                   // string
            IsRequired = required;                 // bool
            DefaultValue = defaultValue;           // any since parser can contain a option of type any
            HelpText = helpText;                   // string
            ErrorMessage = errorMessage;           // string
        }
    }

}