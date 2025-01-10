using System;
using System.Linq;

namespace EasyParser.Core
{
    /// <summary>
    /// Base class for defining shared properties between options and verbs.
    /// Only contains very generic attributes like <see cref="HelpText"/> or 
    /// <see cref="ErrorMessage"/>.
    /// </summary>
    public abstract class BaseAttribute : Attribute
    {
        #region FieldProperties
        /// <summary>
        /// defines the minimum length for a string to be considered as an alias for an attribute
        /// </summary>
        protected static readonly int MinThresholdForAliasLength = 2;

        /// <summary>
        /// Backing field for the helpText. 
        /// </summary>
        /// <remarks>
        /// Should be exclusively set using <see cref="HelpText"/>
        /// </remarks>
        //this is marked nullable bc the compiler will complain if we dont set val for this directly before we exit the constructor but we do it through HelpText with filtering
        [Validated] private string? _helpText;

        /// <summary>
        /// Backing field for the error message. 
        /// </summary>
        /// <remarks>
        /// Should be exclusively set using <see cref="ErrorMessage"/>
        /// </remarks>
        [Validated] private string? _errorMessage;

        /// <summary>
        /// Holds the aliases/different name for the same attribute.
        /// For example an attribute with name Play can have aliases Playing and when the
        /// user inputs Playing, it should be smart enough to know that Play was meant since
        /// Playing is an alias for Play
        /// </summary>
        /// <remarks>
        /// the reason we are not directly using <see cref="Aliases"/> and relying on a private value is that if we set Aliases to 
        /// a certain value, and say the user passes Aliases = new[] {"reading", "studying", "s", ""}, then the aliases will be 
        /// reading, studying, s ;but we dont want s to be a long name alias since it is a char
        /// In other words; using auto property helps add logic for validation which wouldnt be possible if we just used <see cref="_aliases"/>
        /// </remarks>
        //this is marked nullable bc the compiler will complain if we dont set val for this directly before we exit the constructor but we do it through Aliases with filtering
        [Validated] private string[]? _aliases;
        #endregion

        #region AutoProperties
        /// <summary>
        /// Auto property for <see cref="_helpText"/> with set validation
        /// </summary>
        public string HelpText
        {
            get => _helpText ?? string.Empty;
            set => _helpText = string.IsNullOrEmpty( value ) ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Auto property for <see cref="_errorMessage"/> with set validation
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage ?? string.Empty;
            set => _errorMessage = string.IsNullOrEmpty( value ) ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Gets and sets the aliases for the option or verb.
        /// Be careful however, invalid Aliases will be discarded.
        /// </summary>
        /// <remarks>
        /// An Alias is invalid when its length, excluding whitespaces, is less than 2
        ///</remarks>
        public string[] Aliases
        {
            get => _aliases ?? Array.Empty<string>();
            set
            {
                var validAliases = value.Where( alias => !string.IsNullOrWhiteSpace( alias ) && alias.Length >= MinThresholdForAliasLength ).ToArray();
                var discardedAliases = value.Where( alias => alias.Length < MinThresholdForAliasLength ).ToArray();

                _aliases = validAliases;

                if( discardedAliases.Length > 0 )
                {
                    Logger.Debug( $"Some aliases were discarded because they were either empty or their length was less than the defined threshold ({MinThresholdForAliasLength}): {string.Join( ", ", discardedAliases )}" );
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttribute"/> class.
        /// </summary>
        /// <param name="helpText">The help text for the command.</param>
        /// <param name="errorMessage"> The error message to show in case of errors related generally to this attribute.</param>
        /// <param name="aliases">The aliases for the attribute used.</param>
        protected BaseAttribute(
            string helpText,
            string errorMessage,
            params string[] aliases
        )
        {
            HelpText = helpText;
            ErrorMessage = errorMessage;
            Aliases = aliases;
        }
        #endregion

        #region Misc
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"\n{nameof( BaseAttribute )}: \n" +
                $"\t{nameof( HelpText )}: {HelpText}, \n" +
                $"\t{nameof( ErrorMessage )}: {ErrorMessage}, \n" +
                $"\t{nameof( Aliases )}: {string.Join( ", ", Aliases )}\n";
        }
        #endregion
    }
}
