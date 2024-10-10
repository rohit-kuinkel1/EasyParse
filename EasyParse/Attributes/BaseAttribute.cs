using System;

namespace EasyParse.Core
{
    /// <summary>
    /// Base class for defining shared properties between options and verbs.
    /// </summary>
    internal abstract class BaseAttribute : Attribute
    {
        /// <summary>
        /// Gets the long name for the command.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Gets or sets the help text for the command.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the command is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttribute"/> class.
        /// </summary>
        /// <param name="longName">The long name of the command.</param>
        /// <param name="isRequired">Indicates whether the command is required.</param>
        /// <param name="helpText">The help text for the command.</param>
        protected BaseAttribute(
            string longName,
            bool isRequired = false,
            string helpText = "" )
        {
            LongName = longName;
            IsRequired = isRequired;
            HelpText = helpText;
        }
    }
}
