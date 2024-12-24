using System;

namespace EasyParser.Core
{
    /// <summary>
    /// Attribute for defining command-line verbs.
    /// Verbs represent actions that a user can execute.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false )]
    public sealed class VerbAttribute : BaseAttribute
    {
        /// <summary>
        /// Auto property for the short name for the option (single-character).
        /// </summary>
        public char ShortName { get; set; }

        /// <summary>
        /// Auto property for the long name given to this verb.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// Auto property that specifies the necessity of the attribute.
        /// When set to <see langword="true"/> and not provided, will throw an <see cref="Exception"/>
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerbAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the verb.</param>
        /// <param name="longName">The long name of the verb.</param>
        /// <param name="isRequired">Indicates whether the verb is required.</param>
        /// <param name="helpText">The help text for the verb.</param>
        /// <param name="aliases">The aliases for the verb.</param>
        public VerbAttribute(
            char shortName,
            string longName,
            bool isRequired = false,
            string helpText = "",
            params string[] aliases
        )
            : base( helpText, aliases )
        {
            ShortName = shortName;
            LongName = longName;
            Required = isRequired;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="VerbAttribute"/> instance,
        /// including its short name, long name, and whether it is required.
        /// </summary>
        /// <returns>A string representing the verb attribute.</returns>
        public override string ToString()
        {
            return
                $"\nVerbAttribute: \n" +
                $"LongName:{LongName}, \n" +
                $"Short Name:{ShortName}, \n" +
                $"Required: {Required}\n";
        }
    }
}
