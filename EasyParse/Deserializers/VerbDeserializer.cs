using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParser.Utility;

namespace EasyParser.Core
{
    #region UnderConstruction
    /// <summary>
    /// 
    /// </summary>
    public static class VerbDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<VerbDefinition> DeserializeVerbs()
        {
            try
            {
                var verbDefinitions = new List<VerbDefinition>();

                // Get all types in the current assembly that have the VerbAttribute
                var verbTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where( type => type.GetCustomAttributes( typeof( VerbAttribute ), false ).Any() )
                    .ToList();

                // Validate that verbTypes is not null or empty
                _ = Utility.Utility.NotNullValidation( verbTypes, true );

                foreach( var type in verbTypes )
                {
                    // Get the VerbAttribute
                    var verbAttribute = type.GetCustomAttributes( typeof( VerbAttribute ), false )
                        .Cast<VerbAttribute>()
                        .FirstOrDefault();

                    // Validate that verbAttribute is not null
                    _ = Utility.Utility.NotNullValidation( verbAttribute, true );

                    // Get options defined in the class
                    var options = GetOptions( type );

                    verbDefinitions.Add( new VerbDefinition( type, verbAttribute!, options ) );
                }

                return verbDefinitions;
            }
            catch( Exception ex ) when( ex is NullException )
            {
                Console.WriteLine(ex.Message );
                return Enumerable.Empty<VerbDefinition>();
            }
        }

        private static List<Option> GetOptions( Type verbType )
        {
            var options = new List<Option>();

            // Get properties of the verb type that have OptionAttribute
            var properties = verbType.GetProperties( BindingFlags.Public | BindingFlags.Instance );
            foreach( var prop in properties )
            {
                var optionAttr = prop.GetCustomAttributes( typeof( OptionsAttribute ), false )
                                     .Cast<OptionsAttribute>()
                                     .FirstOrDefault();

                if( optionAttr != null )
                {
                    options.Add( new Option( prop, optionAttr ) );
                }
            }

            return options;
        }
    }

    // Example verb definition to hold the class type, verb attribute, and options
    /// <summary>
    /// 
    /// </summary>
    public class VerbDefinition
    {
        public Type VerbType { get; }
        public VerbAttribute VerbAttribute { get; }
        public List<Option> Options { get; }

        public VerbDefinition( Type verbType, VerbAttribute verbAttribute, List<Option> options )
        {
            VerbType = verbType;
            VerbAttribute = verbAttribute;
            Options = options;
        }
    }
    #endregion
}
