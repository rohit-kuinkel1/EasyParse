using System;
using System.Linq;

namespace EasyParser.Core
{
    /// <summary>
    /// Base class for defining shared properties between options and verbs.
    /// Only contains very very generic attributes like <see cref="HelpText"/>.
    /// </summary>
    public abstract class BaseAttribute : Attribute
    {
        /// <summary>
        /// defines the minimum length for a string to be considered as an alias for an attribute
        /// </summary>
        private static readonly int ThresholdForAliasLength = 2;

        /// <summary>
        /// Gets or sets the help text for the command.
        /// </summary>
        public string HelpText { get; set; }

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
        //this is marked nullable bc the compiler will complain if we dont set val for this directly before we exit it but we do it through Aliases with filtering
        private string[]? _aliases;

        /// <summary>
        /// Gets the aliases for the option or verb.
        /// </summary>
        public string[] Aliases
        {
            get => _aliases ?? Array.Empty<string>();
            set
            {
                var validAliases = value.Where( alias => !string.IsNullOrWhiteSpace( alias ) && alias.Length >= ThresholdForAliasLength ).ToArray();
                var discardedAliases = value.Where( alias => alias.Length < ThresholdForAliasLength ).ToArray();

                _aliases = validAliases;

                if( discardedAliases.Length > 0 )
                {
                    Logger.Debug( $"Some aliases were discarded because they were either empty or their length was less than the defined threshold ({ThresholdForAliasLength}): {string.Join( ", ", discardedAliases )}" );
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttribute"/> class.
        /// </summary>
        /// <param name="helpText">The help text for the command.</param>
        /// <param name="aliases">The aliases for the attribute used.</param>
        protected BaseAttribute(
            string helpText,
            params string[] aliases
        )
        {
            HelpText = helpText;
            Aliases = aliases;
        }
    }
}
