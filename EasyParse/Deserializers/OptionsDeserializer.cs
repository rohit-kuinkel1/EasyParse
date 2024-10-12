using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParser.Utility;

namespace EasyParser.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class OptionsDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsType"></param>
        /// <returns></returns>
        public static IEnumerable<OptionStore> DeserializeOptions( Type optionsType )
        {
            try
            {
                _ = Utility.Utility.NotNullValidation( optionsType, true );

                // Get all public properties in the specified type
                var properties = optionsType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

                //throw if properties was null
                _ = Utility.Utility.NotNullValidation( properties, true );

                var optionDefinitions = new List<OptionStore>();
                foreach( var property in properties )
                {
                    // Check if the property has the OptionsAttribute
                    var optionsAttribute = property.GetCustomAttribute<OptionsAttribute>();
                    if( optionsAttribute != null )
                    {
                        var optionDef = new OptionStore( property, optionsAttribute );
                        optionDefinitions.Add( optionDef );
                    }
                }

                return optionDefinitions;
            }
            catch( Exception ex ) when( ex is NullException )
            {
                Console.WriteLine( ex.Message );
                return Enumerable.Empty<OptionStore>();
            }
        }
    }
}