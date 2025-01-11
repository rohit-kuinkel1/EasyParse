using System;

namespace EasyParser.Core
{
    /// <summary>
    /// Attribute for defining command-line verbs.
    /// Verbs represent actions that a user can execute.
    /// This class cannot be inherited.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false )]
    public sealed class VerbAttribute : BaseAttribute
    {
        #region FieldProperties
        /// <summary>
        /// Holds the short name for the verb (single-character).
        /// </summary>
        [Validated] private char shortName;

        /// <summary>
        /// Holds the long name given to this verb.
        /// </summary>
        [Validated] private string longName = string.Empty;

        #endregion

        #region AutoProperties
        /// <summary>
        /// Short name for the verb (single-character).
        /// </summary>
        public char ShortName
        {
            get => shortName;
            set
            {
                if( char.IsWhiteSpace( value ) )
                {
                    throw new ArgumentException( "Short name cannot be a whitespace character and must be a valid character value.", nameof( ShortName ) );
                }

                shortName = value;
            }
        }

        /// <summary>
        /// Long name given to this verb.
        /// </summary>
        public string LongName
        {
            get => longName;
            set
            {
                if( string.IsNullOrWhiteSpace( value ) || string.IsNullOrEmpty( value ) )
                {
                    throw new ArgumentNullException( $"{nameof( LongName )} cannot be null or whitespace.", nameof( LongName ) );
                }

                if( value.Trim().Length < BaseAttribute.MinThresholdForAliasLength )
                {
                    throw new ArgumentException( $"{nameof( LongName )} must have length of at least {MinThresholdForAliasLength} excluding whitespaces", nameof( LongName ) );
                }

                longName = value;
            }
        }

        /// <summary>
        /// Indicates whether the verb is required or not.
        /// </summary>
        [NoValidationRequired] public bool Required { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VerbAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the verb.</param>
        /// <param name="longName">The long name of the verb.</param>
        /// <param name="isRequired">Indicates whether the verb is required.</param>
        /// <param name="helpText">The help text for the verb.</param>
        /// <param name="errorMessage">The error message for general errors during parsing.</param>
        /// <param name="aliases">The aliases for the verb.</param>
        public VerbAttribute(
            char shortName,
            string longName,
            bool isRequired = false,
            string helpText = "",
            string errorMessage = "",
            params string[] aliases
        )
            : base( helpText, errorMessage, aliases )
        {
            ShortName = shortName;
            LongName = longName;
            Required = isRequired;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerbAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the verb.</param>
        /// <param name="longName">The long name of the verb.</param>
        /// <param name="isRequired">Indicates whether the verb is required.</param>
        public VerbAttribute(
            char shortName,
            string longName,
            bool isRequired = false
        )
            : this( shortName, longName, isRequired, "", "", Array.Empty<string>() )
        {
        }
        #endregion

        #region Misc
        /// <summary>
        /// Returns a string representation of the <see cref="VerbAttribute"/> instance,
        /// including its short name, long name, and whether it is required.
        /// </summary>
        /// <returns>A string representing the verb attribute.</returns>
        public override string ToString()
        {
            return
                $"\n{nameof( VerbAttribute )}: \n" +
                $"\t{nameof( LongName )}:{LongName}, \n" +
                $"\t{nameof( ShortName )}:{ShortName}, \n" +
                $"\t{nameof( Required )}:{Required}\n";
        }
        #endregion
    }
}
