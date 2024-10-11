using System;
using System.Collections.Generic;

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
        /// Gets or sets the aliases for the option or verb.
        /// </summary>
        public IEnumerable<string> Aliases { get; }

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
