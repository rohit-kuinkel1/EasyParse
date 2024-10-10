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
        /// Gets the description of the verb.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerbAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the verb.</param>
        /// <param name="isRequired">Indicates whether the verb is required.</param>
        /// <param name="description">The description of the verb.</param>
        /// <param name="helpText">The help text for the verb.</param>
        public VerbAttribute(
            string name,
            bool isRequired = false,
            string description = "",
            string helpText = ""
        ) 
            : base( name, isRequired, helpText )
        {
            Description = description;
        }
    }
}
