using System;
using System.Collections.Generic;

namespace EasyParser.Core
{
    /// <summary>
    /// Represents a command-line verb, including its associated options.
    /// </summary>
    public sealed class VerbStore
    {
        /// <summary>
        /// Gets the type of the verb.
        /// </summary>
        public Type VerbType { get; }

        /// <summary>
        /// Gets the attribute that defines the verb.
        /// </summary>
        public VerbAttribute VerbAttribute { get; }

        /// <summary>
        /// Gets the list of options associated with the verb.
        /// </summary>
        public List<OptionStore> Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerbStore"/> class.
        /// </summary>
        /// <param name="verbType">The type of the verb.</param>
        /// <param name="verbAttribute">The attribute that defines the verb.</param>
        /// <param name="options">The list of options associated with the verb.</param>
        public VerbStore( Type verbType, VerbAttribute verbAttribute, List<OptionStore> options )
        {
            VerbType = verbType;
            VerbAttribute = verbAttribute;
            Options = options;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="VerbStore"/> instance, including
        /// the verb type, verb attribute details, and associated options.
        /// </summary>
        /// <returns>A string representing the verb store.</returns>
        public override string ToString()
        {
            var optionsSummary = string.Join( ", ", Options );
            return $"Verb: {VerbAttribute.LongName} (Short Name: {VerbAttribute.ShortName}, " +
                   $"Is Required: {VerbAttribute.IsRequired}) " +
                   $"Options: [{optionsSummary}]";
        }
    }
}