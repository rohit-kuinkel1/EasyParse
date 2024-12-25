using System;
using System.Collections.Generic;

namespace EasyParser.Core
{
    /// <summary>
    /// Represents a command-line verb, including its associated options.
    /// </summary>
    public sealed class Verb
    {
        /// <summary>
        /// Gets the type of the verb.
        /// </summary>
        public Type? VerbType { get; set; }

        /// <summary>
        /// Gets the attribute that defines the verb.
        /// </summary>
        public VerbAttribute? VerbAttribute { get; set; }

        /// <summary>
        /// Gets the list of options associated with the verb.
        /// </summary>
        public ICollection<Option> Options { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Verb"/> class.
        /// </summary>
        /// <param name="verbType">The type of the verb.</param>
        /// <param name="verbAttribute">The attribute that defines the verb.</param>
        /// <param name="options">The list of options associated with the verb.</param>
        public Verb( Type? verbType, VerbAttribute? verbAttribute, ICollection<Option>? options = null )
        {
            VerbType = verbType;
            VerbAttribute = verbAttribute;
            Options = options ?? new List<Option>();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Verb"/> instance, including
        /// the verb type, verb attribute details, and associated options.
        /// </summary>
        /// <returns>A string representing the verb store.</returns>
        public override string ToString()
        {
            var optionsSummary = string.Join( ", ", Options );
            return 
                $"\nVerbStore: \n" +
                $"LongName:{VerbAttribute?.LongName}, \n" +
                $"ShortName:{VerbAttribute?.ShortName}, \n" +
                $"Required:{VerbAttribute?.Required} \n" +
                $"Options:[{optionsSummary}]\n";
        }
    }
}