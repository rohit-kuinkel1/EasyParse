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
        /// Gets or sets the help text for the command.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// the reason we are not directly using <see cref="Aliases"/> and relying on a private value is that if we set Aliases to 
        /// a certain value, and say the user passes Aliases = new[] {"reading", "studying", "s", ""}, then the aliases will be 
        ///  reading, studying, s ;but we dont want s to be a long name alias since it is a char
        /// </summary>
        private string[]? _aliases;

        /// <summary>
        /// Gets the aliases for the option or verb.
        /// </summary>
        public string[] Aliases
        {
            get => _aliases ?? Array.Empty<string>();
            set
            {
                _aliases = value.Where( alias => alias.Length >= 2 ).ToArray();
                Logger.Debug( "Some aliases were discarded because they were either empty or their length was less than the defined threshold 2." );
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
