using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyParse.Core
{
    /// <summary>
    /// Denotes the store to house the deserialized options present in this assembly
    /// </summary>
    public sealed class Option
    {
        /// <summary>
        /// Gets the property info of the property in the class T, which was decorated with [Options]
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the OptionsAttribute associated with the option.
        /// </summary>
        public OptionsAttribute OptionsAttribute { get; }

        /// <summary>
        /// Gets the SettingsAttribute associated with the option.
        /// </summary>
        public SettingsAttribute? SettingsAttribute { get; }

        /// <summary>
        /// represents the mutual attributes associated with a specific option.
        /// Note: storing a <see cref="List{MutualAttribute}"/> because a single property in the original class can have multiple mutual attributes
        /// attached to it. <see cref="MutualAttribute"/> where AllowMultiple = true is set
        /// </summary>
        public List<MutualAttribute> MutualAttributes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="property">The PropertyInfo of the option.</param>
        /// <param name="optionsAttribute">The OptionsAttribute associated with the option.</param>
        public Option( PropertyInfo property, OptionsAttribute optionsAttribute )
        {
            OptionsAttribute = optionsAttribute;

            Property = property;
            MutualAttributes = property.GetCustomAttributes<MutualAttribute>().ToList() ;
            SettingsAttribute = property.GetCustomAttribute<SettingsAttribute>();
        }

        /// <summary>
        /// Returns a string representation of the OptionStore.
        /// </summary>
        /// <returns>A string that represents the current OptionStore.</returns>
        public override string ToString()
        {
            return
                $"\n\tOptionStore: \n" +
                $"\t\tName: {Property.Name}\n" +
                $"\t\t{OptionsAttribute}\n" +
                $"\t\tSettings: {SettingsAttribute}\n" +
                $"\t\tMutual Attributes: {string.Join( "\t\t\t", MutualAttributes )}\n";
        }
    }
}
