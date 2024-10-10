using System;

namespace EasyParse.Core
{
    /// <summary>
    /// Attribute for defining command-line verbs.
    /// Verbs represent actions that a user can execute.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false )]
    internal sealed class VerbAttribute : BaseAttribute
    {

        /// <summary>
        /// Gets the short name for the option (single-character).
        /// </summary>
        public char ShortName { get; }

        /// <summary>
        /// Gets the long name given to this verb
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Specifies the necessity of the attribute
        /// </summary>
        public bool IsRequired { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="VerbAttribute"/> class.
        /// </summary>
        /// <param name="shortName"> The short name of the verb.</param>
        /// <param name="longName">The long name of the verb.</param>
        /// <param name="isRequired">Indicates whether the verb is required.</param>
        /// <param name="helpText">The help text for the verb.</param>
        /// <param name="aliases"> The aliases for the verb</param>
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
            IsRequired = isRequired;

        }
    }
}
