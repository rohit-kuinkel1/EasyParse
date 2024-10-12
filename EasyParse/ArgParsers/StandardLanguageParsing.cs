using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParser.Core;

namespace EasyParser.Parsing
{
    /// <summary>
    /// This class in contrary to <see cref="NaturalLanguageParsing"/>, aims to parse the args provided to EasyParser where the args are passed in the standard flow
    /// for instance: 
    /// addFile --name Text123.txt --filePath D:/git/Tools/ --smallerThan 5KB ......
    /// af -n Text123.txt -f D:/git/Tools/ -s 5KB ......
    /// </summary>
    internal class StandardLanguageParsing : IParsing
    {
        /// <summary>
        /// Stores all the propertyInfos for the classes marked with <see cref="VerbAttribute"/>
        /// </summary>
        private PropertyInfo[]? _propertyInfos;

        /// <summary>
        /// Container to store the <see cref="VerbAttribute"/> and its related <see cref="OptionsAttribute"/>
        /// </summary>
        private VerbStore? _verbStore;

        /// <summary>
        /// Denotes the prefix for longNames.
        /// </summary>
        private static readonly string LongNamePrefix = "--";

        /// <summary>
        /// Denotes the prefix for shortNames.
        /// </summary>

        private static readonly char ShortNamePrefix = '-';

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        /// <param name="type"></param>
        public bool Parse( string[] args, Type? type = null )
        {
            Logger.Debug( "Entering StandardLanguageParsing.Parse(string[], Type?)" );
            _ = EasyParser.Utility.Utility.NotNullValidation( args );

            _verbStore = new VerbStore( type, null, new List<OptionStore>() );  // If type is null, we will reflect the whole assembly later
            if( EasyParser.Utility.Utility.NotNullValidation( type, false ) )  //we dont fail if the provided type is null since ^^^^^^^^^^^
            {
                _ = TypeIsInstantiable( type, throwIfNotInstantiable: true );
                var instance = Activator.CreateInstance( type );

                // retrieve and store all the public and non-public properties marked with OptionsAttribute
                _propertyInfos = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
                LogDebugPotentialNonPublicPropertyMarkedWithOptionsAttribute();
                var publicPropertiesWithOptionsAttribute = GetPropertyBy( BindingFlags.Public | BindingFlags.Instance );

                // Now fill the VerbStore with public properties
                foreach( var property in publicPropertiesWithOptionsAttribute )
                {
                    var optionsAttribute = Attribute.GetCustomAttribute( property, typeof( OptionsAttribute ) ) as OptionsAttribute;
                    if( EasyParser.Utility.Utility.NotNullValidation( optionsAttribute, throwIfNull: false ) )
                    {
                        var optionStore = new OptionStore( property, optionsAttribute );
                        _verbStore.Options.Add( optionStore );
                    }
                }

                // Parse the provided args
                if( !ParseOptions( args, _verbStore, instance ) )
                {
                    return false;
                }
            }
            else
            {
                // If type is null, reflect over the entire assembly
                Logger.Debug( "Since type was not provided for EasyParse.Parse()" );
                var assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetTypes();

                foreach( var currentType in types )
                {
                    if( TypeIsInstantiable( currentType, false ) )
                    {
                        // Create an instance of each type to populate
                        var instance = Activator.CreateInstance( currentType );
                        var properties = GetPropertyBy( BindingFlags.Public | BindingFlags.Instance );

                        // Fill the VerbStore with options
                        foreach( var property in properties )
                        {
                            var optionsAttribute = Attribute.GetCustomAttribute( property, typeof( OptionsAttribute ) ) as OptionsAttribute;
                            if( Utility.Utility.NotNullValidation( optionsAttribute, throwIfNull: false ) )
                            {
                                var optionStore = new OptionStore( property, optionsAttribute );
                                _verbStore.Options.Add( optionStore );
                            }
                        }

                        // Perform parsing logic
                        if( ParseOptions( args, _verbStore, instance ) )
                        {
                            return true; // Parsing successful for one of the types
                        }
                    }
                }

                Logger.Error( "No valid verb(s) found for the provided arguments. " +
                    "Do you have at least one non-static public class marked with '[Verb(....)]' or '[VerbAttribute(....)]' ??" );
                return false;
            }
            Logger.Info(_verbStore.ToString() );
            return true;
        }

        /// <summary>
        /// validates that the provided <paramref name="type"/> is not a static or an abstract class since they cannot be instantiated.
        /// </summary>
        /// <exception cref="EasyParser.Utility.IllegalOperation"></exception>
        private bool TypeIsInstantiable( Type? type, bool throwIfNotInstantiable = true )
        {
            Logger.Debug( "Entering StandardLanguageParsing.TypeIsInstantiable( Type?, bool )" );
            if( type!.IsAbstract && type!.IsSealed )
            {
                return throwIfNotInstantiable
                    ? throw new EasyParser.Utility.IllegalOperation( $"The provided type {type.FullName} is a static class hence cannot be instantiated." +
                        $" Please remove the static keyword and try again." )
                    : false;
            }
            else if( type!.IsAbstract )
            {
                return throwIfNotInstantiable
                    ? throw new EasyParser.Utility.IllegalOperation( $"The provided type {type.FullName} is an abstract class hence cannot be instantiated." +
                        $" Please remove the abstract keyword and try again." )
                    : false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// If the log level permits and some non-public properties are marked with <see cref="OptionsAttribute"/>, then let the user know about it using <see cref="Logger.Debug(string)"/>
        /// </summary>
        private void LogDebugPotentialNonPublicPropertyMarkedWithOptionsAttribute()
        {
            Logger.Debug( "Entering StandardLanguageParsing.LogDebugPotentialNonPublicPropertyMarkedWithOptionsAttribute()" );
            var nonPublicPropertiesWithOptionsAttribute = GetPropertyBy( BindingFlags.NonPublic | BindingFlags.Instance );
            if( nonPublicPropertiesWithOptionsAttribute.Length != 0 )
            {
                foreach( var property in nonPublicPropertiesWithOptionsAttribute )
                {
                    Logger.Debug( $"Property '{property.Name}' is marked with OptionsAttribute but is not set to public." +
                        $"Knowingly avoiding non-public property {property.Name}..." );
                }
            }
            return;
        }

        /// <summary>
        /// retrieves the properties marked with the <see cref="OptionsAttribute"/> in the type provided to <see cref="Parse(string[],Type?)"/>.
        /// Param <paramref name="bindingFlags"/> is used to retrieve the properties with specific access specifiers.
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns> Array of <see cref="PropertyInfo"/></returns>
        /// <exception cref="Utility.NullException">Thrown when the <see cref="_propertyInfos"/> has no properties in it.</exception>
        private PropertyInfo[] GetPropertyBy( BindingFlags bindingFlags )
        {
            Logger.Debug( "Entering StandardLanguageParsing.GetPropertyBy( BindingFlags )" );
            if( Utility.Utility.NotNullValidation( _propertyInfos ) )
            {
                // Filter and return properties that are marked with OptionsAttribute
                return _propertyInfos
                    .Where( property => Attribute.IsDefined( property, typeof( OptionsAttribute ) ) &&
                                      ( bindingFlags & BindingFlags.Public ) != 0 && property.GetGetMethod( true )?.IsPublic == true ||
                                      ( bindingFlags & BindingFlags.NonPublic ) != 0 && property.GetGetMethod( true )?.IsFamily == true ||
                                      ( bindingFlags & BindingFlags.Instance ) != 0 && !property.GetGetMethod( true )?.IsStatic == true )
                    .ToArray();
            }
            return [];
        }


        // Helper method to handle option parsing logic
        private bool ParseOptions( string[] args, VerbStore verbStore, object? instance )
        {
            Logger.Debug( "Entering StandardLanguageParsing. ParseOptions( string[], VerbStore, object? )" );
            _ = EasyParser.Utility.Utility.NotNullValidation( args );
            _ = EasyParser.Utility.Utility.NotNullValidation( verbStore );
            _ = EasyParser.Utility.Utility.NotNullValidation( instance );

            var parsedOptions = new Dictionary<string, object>();

            for( var i = 0; i < args.Length; i++ )
            {
                // Check for long option
                if( args[i].StartsWith( LongNamePrefix ) )
                {
                    var optionName = args[i].Substring( 2 ); // remove '--'
                    if( i + 1 < args.Length && !args[i + 1].StartsWith( LongNamePrefix ) )
                    {
                        parsedOptions[optionName] = args[++i]; // advance to the value
                    }
                }
                // Check for short option
                else if( args[i].StartsWith( ShortNamePrefix ) )
                {
                    var optionName = args[i].Substring( 1 ); // remove '-'
                    if( i + 1 < args.Length && !args[i + 1].StartsWith( ShortNamePrefix ) )
                    {
                        parsedOptions[optionName] = args[++i]; // advance to the value
                    }
                }
            }

            // Populate the instance from parsed options using verbStore
            foreach( var optionStore in verbStore.Options )
            {
                var optionAttr = optionStore.OptionsAttribute;
                if( parsedOptions.TryGetValue( optionAttr.LongName, out var value ) ||
                    parsedOptions.TryGetValue( optionAttr.ShortName.ToString(), out value ) )
                {
                    // Convert the value to the appropriate property type and set it
                    optionStore.Property.SetValue( instance, Convert.ChangeType( value, optionStore.Property.PropertyType ) );
                }
                else if( optionAttr.Required )
                {
                    // Handle missing required options
                    Logger.Error( $"Option {optionAttr.LongName} is marked as required but could not be parsed." );
                    return false;
                }
            }

            return true;
        }
    }
}
