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
        /// Stores all the propertyInfos for the classes marked with <see cref="VerbAttribute"/> regardless of the binding flags.
        /// </summary>
        private PropertyInfo[]? _allPropertyInfosFromType;

        /// <summary>
        /// Container to store the <see cref="VerbAttribute"/> and its related <see cref="OptionsAttribute"/>.
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
        public ParsingResult<T> Parse<T>( string[] args ) where T : new()
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.Parse<string[]>(args) with args " +
                $"with Len:{args.Length} and type {typeof( T ).FullName}" );
            _ = EasyParser.Utility.Utility.NotNullValidation( args );

            // Initialize VerbStore
            _verbStore = new VerbStore( typeof( T ), null, new List<OptionStore>() );

            // Create an instance of T
            var instance = new T();

            // Reflect to get all properties of the type
            _allPropertyInfosFromType = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );

            // Log potential non-public properties marked with OptionsAttribute
            LogPotentialNonPublicPropertyMarkedWithOptionsAttribute();

            // Get public properties with OptionsAttribute
            var publicPropertiesWithOptionsAttribute = GetPropertyBy( BindingFlags.Public | BindingFlags.Instance );

            // Store properties marked with OptionsAttribute
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
                return new ParsingResult<T>( false, "Parsing failed.", default );
            }

            // Return success with the populated instance
            return new ParsingResult<T>( true, null, instance );
        }

        /// <summary>
        /// validates that the provided <paramref name="type"/> is not a static or an abstract class since they cannot be instantiated.
        /// </summary>
        /// <exception cref="EasyParser.Utility.IllegalOperation"></exception>
        private bool TypeIsInstantiable(
            Type? type,
            bool throwIfNotInstantiable = true )
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.TypeIsInstantiable( Type?, bool ) with " +
                $"type {type?.FullName} and throwIfNotInstantiable {throwIfNotInstantiable}" );
            if( type!.IsAbstract && type!.IsSealed )
            {
                var message = $"The provided type {type.FullName} is a static class hence cannot be instantiated." +
                        $" Please remove the static keyword and try again.";
                Logger.Debug( message );

                return throwIfNotInstantiable
                    ? throw new EasyParser.Utility.IllegalOperation( message )
                    : false;
            }
            else if( type!.IsAbstract )
            {
                var message = $"The provided type {type.FullName} is an abstract class hence cannot be instantiated." +
                        $" Please remove the abstract keyword and try again.";
                Logger.Debug( message );

                return throwIfNotInstantiable
                    ? throw new EasyParser.Utility.IllegalOperation( message )
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
        private void LogPotentialNonPublicPropertyMarkedWithOptionsAttribute()
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.LogDebugPotentialNonPublicPropertyMarkedWithOptionsAttribute()" );
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
        /// <exception cref="Utility.NullException">Thrown when the <see cref="_allPropertyInfosFromType"/> has no properties in it.</exception>
        private PropertyInfo[] GetPropertyBy( BindingFlags bindingFlags )
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.GetPropertyBy(BindingFlags) with bindingFlags {bindingFlags}" );

            if( Utility.Utility.NotNullValidation( _allPropertyInfosFromType ) )
            {
                return _allPropertyInfosFromType
                    .Where( property =>
                        // Check if the property has the OptionsAttribute
                        Attribute.IsDefined( property, typeof( OptionsAttribute ) )

                        // Apply binding flags for public or non-public instance properties
                        && ( property.GetGetMethod( true )?.IsPublic == true && ( bindingFlags & BindingFlags.Public ) != 0
                            || property.GetGetMethod( true )?.IsFamily == true && ( bindingFlags & BindingFlags.NonPublic ) != 0 )
                        && ( bindingFlags & BindingFlags.Instance ) != 0 // Ensure the property is an instance property
                    ).ToArray();
            }
            return [];
        }


        // Helper method to handle option parsing logic
        private bool ParseOptions(
            string[] args,
            VerbStore verbStore,
            object? instance )
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing. ParseOptions( string[], VerbStore, object? ) " +
                $"with args with Len:{args.Length}, verbStore:{verbStore} and instance {instance?.ToString()}" );
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
                    Logger.Critical( $"Option {optionAttr.LongName} is marked as required but could not be parsed." );
                    return false;
                }
            }

            return true;
        }
    }
}
