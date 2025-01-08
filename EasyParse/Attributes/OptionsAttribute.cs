using System;

namespace EasyParser.Core
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
        public char ShortName { get; set; }

        /// <summary>
        /// Gets the long name given to this option.
        /// </summary>
        public string LongName { get; set; }

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
            //ensure shortName is not null/empty/whitespace character
            ShortName = char.IsWhiteSpace( shortName ) || shortName == default
                ? throw new ArgumentException( "Short name cannot just be whitespaces or default char value", nameof( shortName ) )
                : shortName;

            //longName must be non-null, non-empty, non-whitespace, and >= 2 chars
            LongName = string.IsNullOrEmpty( longName )
                ? throw new ArgumentNullException( nameof( longName ) )
                : longName.Length >= 2 && !string.IsNullOrWhiteSpace( longName )
                    ? longName
                    : throw new ArgumentException( "Long name must be at least 2 characters and not whitespace", nameof( longName ) );

            //bool cant be null since its not a ref type and we didnt say its nullable
            Required = isRequired;
            Default = defaultValue;

            //ensure the error msg is not just whitespace if it was provided
            ErrorMessage = !string.IsNullOrEmpty( errorMessage ) && string.IsNullOrWhiteSpace( errorMessage )
                ? throw new ArgumentException( "Error message cannot just be whitespaces", nameof( errorMessage ) )
                : errorMessage;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="OptionsAttribute"/> instance,
        /// including its short name, long name, whether it is required, default value, help text, and error message.
        /// </summary>
        /// <returns>A string representing the options attribute.</returns>
        public override string ToString()
        {
            return
                $"\n\t\tOptionsAttribute: \n" +
                $"\t\t\tLongName:{LongName}, \n" +
                $"\t\t\tShortName:{ShortName}, \n" +
                $"\t\t\tRequired:{Required}, \n" +
                $"\t\t\tDefaultValue:{Default}, \n" +
                $"\t\t\tHelpText:{HelpText}, \n" +
                $"\t\t\tErrorMessage:{ErrorMessage} \n";
        }
    }
}
