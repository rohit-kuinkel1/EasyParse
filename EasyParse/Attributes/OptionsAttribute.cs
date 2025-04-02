using System;

namespace EasyParse.Core
{
    /// <summary>
    /// Attribute for defining command line options.
    /// This class cannot be inherited.
    /// <see cref="OptionsAttribute"/> is meant to be hung on class properties to define a class property as an option for a verb
    /// For example in git commit -m, commit would be the "verb" and "-m" would be the option.
    /// In the code below -s or --stdin would be the option
    /// <code>
    ///  [Options('s', "stdin", Default = false, Required = false, HelpText = "Read from stdin", Aliases = new[] { "standardin", "stdinput" })]
    ///  public bool Stdin { get; set; }
    /// </code>
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    public sealed class OptionsAttribute : BaseAttribute
    {
        #region FieldProperties
        /// <summary>
        /// holds the short name for this <see cref="OptionsAttribute"/>
        /// </summary>
        [Validated] private char _shortName;

        /// <summary>
        /// holds the long name for this <see cref="OptionsAttribute"/>
        /// </summary>
        [Validated] private string _longName = string.Empty;
        #endregion

        #region AutoProperties
        /// <summary>
        /// Gets or sets the short name for the option (single-character).
        /// Validates the provided value to ensure it is not whitespace or default char value.
        /// </summary>
        public char ShortName
        {
            get => _shortName;
            set => _shortName = char.IsWhiteSpace( value ) || value == default
                ? throw new ArgumentException( "Short name cannot just be whitespaces or default char value", nameof( ShortName ) )
                : value;
        }

        /// <summary>
        /// Gets or sets the long name given to this option.
        /// Validates the provided value to ensure it is not null, empty, whitespace, and is at least 2 characters long after using <see cref="string.Trim()"/>.
        /// </summary>
        public string LongName
        {
            get => _longName;
            set => _longName = string.IsNullOrEmpty( value )
                ? throw new ArgumentNullException( nameof( LongName ) )
                : value.Trim().Length >= 2 && !string.IsNullOrWhiteSpace( value )
                    ? value.Trim()
                    : throw new ArgumentException( "Long name must be at least 2 characters excluding whitespaces", nameof( LongName ) );
        }

        /// <summary>
        /// Specifies the necessity of the attribute.
        /// When set to <see langword="true"/>, this option is mandatory which means the parsing will fail
        /// if this option was not provided.
        /// </summary>
        [NoValidationRequired] public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the default value for the option.
        /// Can be anything hence the type <see cref="object"/>
        /// </summary>
        [NoValidationRequired] public object? Default { get; set; }
        #endregion

        #region Constructors
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
        : base( helpText, errorMessage, aliases )
        {
            ShortName = shortName;
            LongName = longName;
            Required = isRequired;
            Default = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name for the option (single character).</param>
        /// <param name="longName">The long name for the option.</param>
        /// <param name="isRequired">Indicates whether the option is required.</param>
        public OptionsAttribute(
            char shortName,
            string longName,
            bool isRequired
        )
        : this( shortName, longName, isRequired, null, "", "", Array.Empty<string>() )
        {
        }
        #endregion

        #region Misc
        /// <summary>
        /// Returns a string representation of the <see cref="OptionsAttribute"/> instance,
        /// including its short name, long name, whether it is required, default value, help text, and error message.
        /// </summary>
        /// <returns>A string representing the options attribute.</returns>
        public override string ToString()
        {
            return
                $"\n\t\t{nameof( OptionsAttribute )}: \n" +
                $"\t\t\t{nameof( LongName )}:{LongName}, \n" +
                $"\t\t\t{nameof( ShortName )}:{ShortName}, \n" +
                $"\t\t\t{nameof( Required )}:{Required}, \n" +
                $"\t\t\t{nameof( Default )}:{Default}, \n" +
                $"\t\t\t{nameof( HelpText )}:{HelpText}, \n" +
                $"\t\t\t{nameof( ErrorMessage )}:{ErrorMessage} \n";
        }
        #endregion
    }
}
