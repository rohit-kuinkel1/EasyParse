using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParser.Core;
using EasyParser.Utility;

namespace EasyParser.Parsing
{
    /// <summary>
    /// This class in contrary to <see cref="NaturalLanguageParsing"/>, aims to parse the args provided to EasyParser where the args are passed in the standard flow
    /// for instance: 
    /// addFile --name Text123.txt --filePath D:/git/Tools/ --smallerThan 5KB ......
    /// a -n Text123.txt -f D:/git/Tools/ -s 5KB ......
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
        private readonly string _longNamePrefix;

        /// <summary>
        /// Denotes the prefix for shortNames.
        /// </summary>
        private readonly char _shortNamePrefix;

        /// <summary>
        /// Default parameterized constructor for <see cref="StandardLanguageParsing"/>
        /// </summary>
        /// <param name="longNamePrefix"></param>
        /// <param name="shortNamePrefix"></param>
        public StandardLanguageParsing(string longNamePrefix = "--", char shortNamePrefix = '-') 
        {
            _longNamePrefix = longNamePrefix;
            _shortNamePrefix = shortNamePrefix;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        public ParsingResult<T> Parse<T>( string[] args ) where T : new()
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.Parse<string[]>(args) with args " +
                $"with Len:{args.Length} and type {typeof( T ).FullName}" );

            _ = EasyParser.Utility.Utility.NotNullValidation( args );
            _verbStore = new VerbStore( typeof( T ), null, new List<OptionStore>() );

            // Check if T is defined with VerbAttribute
            if( typeof( T ).IsDefined( typeof( VerbAttribute ), inherit: false ) )
            {
                //we dont directly instantiate _verbStore here to avoid the pesky possible null refernce warnings
                _verbStore.VerbAttribute = Attribute.GetCustomAttribute( typeof( T ), typeof( VerbAttribute ) ) as VerbAttribute;
            }
            else
            {
                Logger.Debug( $"Type {typeof( T ).FullName} is not marked with VerbAttribute." );
            }

            var instance = new T();

            _allPropertyInfosFromType = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            LogPotentialNonPublicPropertyMarkedWithOptionsAttribute();
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
                return new ParsingResult<T>( false, "Parsing failed.", default! );
            }

            // Return success with the populated instance
            Logger.BackTrace( _verbStore.ToString() );
            return new ParsingResult<T>( true, "status: OK", instance );
        }

        /// <summary>
        /// validates that the provided <paramref name="type"/> is not a static or an abstract class since they cannot be instantiated.
        /// </summary>
        /// <exception cref="EasyParser.Utility.IllegalOperation"></exception>
        private bool TypeIsInstantiable(
            Type type,
            bool throwIfNotInstantiable = true )
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.TypeIsInstantiable( Type?, bool ) with " +
                $"type {type.FullName} and throwIfNotInstantiable {throwIfNotInstantiable}" );
            if( type.IsAbstract && type.IsSealed )
            {
                var message = $"The provided type {type.FullName} is a static class hence cannot be instantiated." +
                        $" Please remove the static keyword and try again.";
                Logger.Debug( message );

                return throwIfNotInstantiable
                    ? throw new EasyParser.Utility.IllegalOperation( message )
                    : false;
            }
            else if( type.IsAbstract )
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
        /// If the log level permits and some non-public properties are marked with <see cref="OptionsAttribute"/>, 
        /// then let the user know about it using <see cref="Logger.Debug(string)"/>
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
        /// retrieves the properties marked with the <see cref="OptionsAttribute"/> in the type provided to <see cref="Parse(string[])"/>.
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


        /// <summary>
        /// Helper method to parse the actual input <paramref name="args"/>
        /// Examples:
        /// apple --banana car --dog elephant --isEdible false --count 10 (ok)
        /// apple --banana --dog elephant --isEdible null --count noCount (wrong because isEdible is bool cant have null, count expects int cant have string)
        /// apple --banana --dog --isEdible true --count 10 (ok, banana and dog will get the default values for the respective types)
        /// apple --banana --dog --isEdible True --count 10.8 (ok, True will be converted to true, 10.8 will be rounded down to 10)
        /// apple --banana Ferrari car super fast --dog elephant with 2 trunks --isEdible false --count 10 (ok, banana will get the whole string just till we reach --, same with dog)
        /// apple --banana Ferrari car super fast --dog elephant with 2 trunks --isEdible false --count "10" (ok, EasyParser will convert the string to an int value
        /// and if its a valid int value, will assign --count with the value)
        /// </summary>
        /// <param name="args"></param>
        /// <param name="verbStore"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private bool ParseOptions(
    string[] args,
    VerbStore verbStore,
    object? instance )
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.ParseOptions(string[], VerbStore, object?) " +
                $"with args Len:{args.Length}, verbStore:{verbStore}, instance {instance?.ToString()}" );

            _ = EasyParser.Utility.Utility.NotNullValidation( args );
            _ = EasyParser.Utility.Utility.NotNullValidation( verbStore );
            _ = EasyParser.Utility.Utility.NotNullValidation( instance );

            var parsedOptions = new Dictionary<string, object>();

            for( var i = 0; i < args.Length; i++ )
            {
                if( args[i].StartsWith( _longNamePrefix ) )
                {
                    var optionName = args[i].Substring( _longNamePrefix.Length );
                    var value = ParseMultiWordValue( args, ref i, _longNamePrefix, _shortNamePrefix );
                    parsedOptions[optionName] = value;
                }
                else if( args[i].StartsWith( _shortNamePrefix ) )
                {
                    var optionName = args[i].Substring( 1 );
                    var value = ParseMultiWordValue( args, ref i, _longNamePrefix, _shortNamePrefix );
                    parsedOptions[optionName] = value;
                }
            }

            // Populate the instance from parsed options using verbStore
            foreach( var optionStore in verbStore.Options )
            {
                var optionAttr = optionStore.OptionsAttribute;
                var aliases = optionAttr.Aliases;

                // Check for LongName, ShortName, and Aliases
                if( parsedOptions.TryGetValue( optionAttr.LongName, out var value ) ||
                    parsedOptions.TryGetValue( optionAttr.ShortName.ToString(), out value ) ||
                    aliases.Any( alias => parsedOptions.TryGetValue( alias, out value ) ) )
                {
                    try
                    {
                        // Convert and assign value to the appropriate type
                        var convertedValue = ConvertToOptionType( value, optionStore.Property.PropertyType, optionStore.OptionsAttribute.LongName );
                        optionStore.Property.SetValue( instance, convertedValue );
                    }
                    catch( Exception ex )
                    {
                        Logger.Critical( $"{ex.Message}" );
                        return false;
                    }
                }
                else if( optionAttr.Required )
                {
                    Logger.Critical( $"Option '{optionAttr.LongName}' is marked as required, but was not provided." +
                        $" Please provide the corresponding value for {optionAttr.LongName} and try again." +
                        $" Defined aliases for '{optionAttr.LongName}' are: {string.Join( ", ", optionAttr.Aliases.Where( alias => !string.IsNullOrEmpty( alias ) ) )} " );
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parses multi-word values until another option or the end of args is found.
        /// </summary>
        /// <param name="args"> the original args passed to <see cref="StandardLanguageParsing"/>.</param>
        /// <param name="index"> the first index after the option.</param>
        /// <param name="longNamePrefix">the prefix symbols for longName.</param>
        /// <param name="shortNamePrefix"> the prefix symbol for shortName.</param>
        private string ParseMultiWordValue(
            string[] args,
            ref int index,
            string longNamePrefix,
            char shortNamePrefix )
        {
            var valueBuilder = new List<string>();
            index++;

            while( index < args.Length && !args[index].StartsWith( longNamePrefix ) && !args[index].StartsWith( shortNamePrefix ) )
            {
                valueBuilder.Add( args[index] );
                index++;
            }

            // Move back by one since we advanced too far
            index--;

            return string.Join( " ", valueBuilder );
        }

        /// <summary>
        /// Converts a value to the expected property type, handling special cases like bool and numeric conversions.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="optionName"> Name of the options for which the processing is being done.</param>
        /// <returns>The converted value.</returns>
        private object ConvertToOptionType( object value, Type targetType, string optionName )
        {
            var valueStr = value.ToString()?.Trim( '"' ) ?? string.Empty; // Trim quotes if present
            if( valueStr.Length == 0 )
            {
                throw new InvalidValueException( $"Missing value for the required parameter '{optionName}', please " +
                    "check that you have values for all the required parameters and try again." );
            }

            // Handle boolean conversion
            if( targetType == typeof( bool ) )
            {
                if( bool.TryParse( valueStr, out bool boolResult ) )
                {
                    return boolResult;
                }

                var errorMessage = $"Invalid boolean value: {valueStr}";
                throw new InvalidValueException( errorMessage );
            }

            if( targetType == typeof( int ) )
            {
                // Attempt to parse as decimal and round down
                if( decimal.TryParse( valueStr, out var decimalResult ) )
                {
                    return (int)Math.Floor( decimalResult );
                }

                var errorMessage = $"Invalid integer value: {valueStr}";
                throw new InvalidValueException( errorMessage );
            }

            // Default conversion for other types
            return Convert.ChangeType( valueStr, targetType );
        }
    }
}
