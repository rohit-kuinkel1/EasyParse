using System.Reflection;

namespace EasyParser.Core
{
    /// <summary>
    /// Denotes the store to house the deserialized options present in this assembly
    /// </summary>
    public sealed class OptionStore
    {
        /// <summary>
        /// Gets the PropertyInfo for the option.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the OptionsAttribute associated with the option.
        /// </summary>
        public OptionsAttribute OptionsAttribute { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionStore"/> class.
        /// </summary>
        /// <param name="property">The PropertyInfo of the option.</param>
        /// <param name="optionsAttribute">The OptionsAttribute associated with the option.</param>
        public OptionStore( PropertyInfo property, OptionsAttribute optionsAttribute )
        {
            Property = property;
            OptionsAttribute = optionsAttribute;
        }

        /// <summary>
        /// Returns a string representation of the OptionStore.
        /// </summary>
        /// <returns>A string that represents the current OptionStore.</returns>
        public override string ToString()
        {
            return 
                $"OptionStore:Name:{Property.Name}" +
                $"{OptionsAttribute.ToString()}";
        }
    }
}
