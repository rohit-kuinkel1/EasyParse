using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParser.Core;
using EasyParser.Enums;
using EasyParser.Utility;

namespace EasyParser.Parsing
{
    /// <summary>
    /// This class is contrary to <see cref="StandardLanguageParsing"/>.
    /// <see cref="NaturalLanguageParsing"/> aims to parse the args provided to <see cref="EasyParse"/> 
    /// where the args are passed in a natural flow of language
    /// for instance: 
    /// addFile where name is Text123.txt filePath is D:/git/Tools/ smallerThan is 5KB
    /// a where n is Text123.txt f is D:/git/Tools/ s is 5KB
    /// </summary>
    internal class NaturalLanguageParsing : IParsing
    {
        /// <summary>
        /// Stores all the propertyInfos for the classes marked with <see cref="VerbAttribute"/> regardless of the binding flags.
        /// </summary>
        private PropertyInfo[]? _allPropertyInfosFromType;

        /// <summary>
        /// Container to store the <see cref="VerbAttribute"/> and its related <see cref="OptionsAttribute"/>.
        /// </summary>
        private VerbStore? _verbStore;

        internal NaturalLanguageParsing()
        {
            _allPropertyInfosFromType = null;
            _verbStore = null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        public ParsingResult<T> Parse<T>( string[] args ) where T : class, new()
        {
            Logger.BackTrace( $"Entering {nameof( NaturalLanguageParsing )}.{nameof( Parse )} with args " +
                $"with Len:{args.Length} and type T as {typeof( T ).FullName}" );

            _ = EasyParser.Utility.Utility.NotNullValidation( args );

            _verbStore = new VerbStore( typeof( T ), null, new List<OptionStore>() );

            //check if T is defined with VerbAttribute
            if( typeof( T ).IsDefined( typeof( VerbAttribute ), inherit: false ) )
            {
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

            foreach( var property in publicPropertiesWithOptionsAttribute )
            {
                var optionsAttribute = Attribute.GetCustomAttribute( property, typeof( OptionsAttribute ) ) as OptionsAttribute;
                if( EasyParser.Utility.Utility.NotNullValidation( optionsAttribute, throwIfNull: false ) )
                {
                    var optionStore = new OptionStore( property, optionsAttribute );
                    _verbStore.Options.Add( optionStore );
                }
            }

            if( !ParseOptions( args, _verbStore, instance ) )
            {
                return new ParsingResult<T>( false, "ERROR", default! );
            }

            Logger.BackTrace( _verbStore.ToString() );
            return new ParsingResult<T>( true, "OK", instance );
        }

        /// <summary>
        /// If the log level permits and some non-public properties are marked with <see cref="OptionsAttribute"/>, 
        /// then let the user know about it using <see cref="Logger.Debug(string)"/>
        /// </summary>
        private void LogPotentialNonPublicPropertyMarkedWithOptionsAttribute()
        {
            Logger.BackTrace( $"Entering NaturalLanguageParsing.LogPotentialNonPublicPropertyMarkedWithOptionsAttribute()" );
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
            Logger.BackTrace( $"Entering NaturalLanguageParsing.GetPropertyBy(BindingFlags) with bindingFlags {bindingFlags}" );

            return Utility.Utility.NotNullValidation( _allPropertyInfosFromType )
                ? _allPropertyInfosFromType
                    .Where( property =>
                        Attribute.IsDefined( property, typeof( OptionsAttribute ) )
                        && ( property.GetGetMethod( true )?.IsPublic == true && ( bindingFlags & BindingFlags.Public ) != 0
                            || property.GetGetMethod( true )?.IsFamily == true && ( bindingFlags & BindingFlags.NonPublic ) != 0 )
                        && ( bindingFlags & BindingFlags.Instance ) != 0
                    ).ToArray()
                : Array.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Helper method to parse the actual input <paramref name="args"/>
        /// Examples:
        /// addFile where name is Text123.txt filePath is D:/git/Tools/ smallerThan is 5KB (ok)
        /// addFile where name is Text123.txt filePath is D:/git/Tools/ isEdible is false count is 10 (ok)
        /// addFile where name is Text123.txt filePath is D:/git/Tools/ isEdible is null count is noCount (wrong because isEdible is bool cant have null, count expects int cant have string)
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
            Logger.BackTrace( $"Entering NaturalLanguageParsing.ParseOptions(string[], VerbStore, object?) " +
                $"with args Len:{args.Length}, verbStore:{verbStore}, instance {instance?.ToString()}" );

            _ = EasyParser.Utility.Utility.NotNullValidation( args );
            _ = EasyParser.Utility.Utility.NotNullValidation( verbStore );
            _ = EasyParser.Utility.Utility.NotNullValidation( instance );

            var parsedOptions = new Dictionary<string, object>();
            var whereIndex = args.Select( ( arg, index ) => new { arg, index } )
             .FirstOrDefault( x => string.Equals( x.arg.ToLowerInvariant(), ParsingKeyword.Where.ToString().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase ) )?.index ?? -1;

            if( whereIndex == -1 )
            {
                Logger.Critical( $"Parsing in a natural flow requires the '{ParsingKeyword.Where}' keyword" );
                return false;
            }

            //first pass: collect all options and their values
            for( var i = whereIndex + 1; i < args.Length; )
            {
                if( i + 2 < args.Length && args[i + 1].ToLowerInvariant() == ParsingKeyword.Is.ToString().ToLowerInvariant() )
                {
                    var optionName = args[i];
                    var value = args[i + 2]; //get the value directly after "is"
                    parsedOptions[optionName] = value;
                    i += 3; //move to next option by skipping current option, "is", and value
                }
                else
                {
                    i++; //skip unknown tokens for better parsing
                }
            }


            foreach( var optionStore in verbStore.Options )
            {
                OptionsAttribute? optionAttr = default;
                string[]? aliases = default;
                try
                {
                    optionAttr = optionStore.OptionsAttribute;
                    aliases = optionAttr.Aliases;

                    if( ( parsedOptions.TryGetValue( optionAttr.LongName, out var value )
                            || parsedOptions.TryGetValue( optionAttr.ShortName.ToString(), out value )
                            || aliases.Any( alias => parsedOptions.TryGetValue( alias, out value ) ) )
                        && Utility.Utility.NotNullValidation( value ) )
                    {
                        if( !ValidateMutualRelationships( verbStore.Options, optionStore, parsedOptions ) )
                        {
                            return false;
                        }

                        var convertedValue = ConvertToOptionType( value, optionStore.Property.PropertyType, optionStore.OptionsAttribute.LongName );
                        optionStore.Property.SetValue( instance, convertedValue );
                    }
                    else if( optionAttr.Required )
                    {
                        Logger.Critical( $"Option '{optionAttr.LongName}' is marked as required, but was not provided." +
                            $" Please provide the corresponding value for {optionAttr.LongName} and try again." +
                            $" Defined aliases for '{optionAttr.LongName}' are: {string.Join( ", ", optionAttr.Aliases.Where( alias => !string.IsNullOrEmpty( alias ) ) )} " );
                        return false;
                    }
                }
                catch( Exception ex ) when( ex is NullException )
                {
                    Logger.Critical( $"Unexpected NullException occurred while trying to parse the optionsAttribute {optionAttr?.LongName}" );
                    return false;
                }
                catch( Exception ex )
                {
                    Logger.Critical( $"{ex.Message}" );
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates the mutual agreement <see cref="MutualAttribute"/> between <see cref="OptionsAttribute"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        private bool ValidateMutualRelationships(
            ICollection<OptionStore> options,
            OptionStore currentOption,
            Dictionary<string, object> parsedOptions )
        {
            Logger.Debug( "Current parsed options:" );
            foreach( var kvp in parsedOptions )
            {
                Logger.Debug( $"Key: {kvp.Key}, Value: {kvp.Value}" );
            }

            foreach( var mutualAttr in currentOption.MutualAttributes )
            {
                foreach( var relatedEntity in mutualAttr.RelatedEntities )
                {
                    var relatedOption = options.FirstOrDefault( o => o.Property.Name == relatedEntity );

                    if( relatedOption != null )
                    {
                        var isCurrentOptionProvided = IsOptionProvided( currentOption, parsedOptions );
                        var isRelatedOptionProvided = IsOptionProvided( relatedOption, parsedOptions );

                        //Logger.Debug( $"Checking mutual relationship:" );
                        //Logger.Debug( $"Current option: {currentOption.Property.Name} ({currentOption.OptionsAttribute.LongName}), Provided: {isCurrentOptionProvided}" );
                        //Logger.Debug( $"Related option: {relatedOption.Property.Name} ({relatedOption.OptionsAttribute.LongName}), Provided: {isRelatedOptionProvided}" );
                        //Logger.Debug( $"Relationship type: {mutualAttr.RelationshipType}" );

                        if( mutualAttr.RelationshipType == MutualType.Inclusive )
                        {
                            //for inclusive relationship, either both should be present or both should be absent
                            if( isCurrentOptionProvided != isRelatedOptionProvided )
                            {
                                Logger.Critical( $"Options {nameof( currentOption.OptionsAttribute.LongName )}:'{currentOption.OptionsAttribute.LongName}' " +
                                    $"and {nameof( relatedOption.OptionsAttribute.LongName )}:'{relatedOption.OptionsAttribute.LongName}' " +
                                    $"were defined to be mutually inclusive to each other. Both must be provided at the same time." );
                                return false;
                            }
                        }
                        else if( mutualAttr.RelationshipType == MutualType.Exclusive && ( isCurrentOptionProvided && isRelatedOptionProvided ) )
                        {
                            Logger.Critical( $"Options {nameof( currentOption.OptionsAttribute.LongName )}:'{currentOption.OptionsAttribute.LongName}' " +
                                $"and {nameof( relatedOption.OptionsAttribute.LongName )}:'{relatedOption.OptionsAttribute.LongName}' " +
                                $"are mutually exclusive to each other. Only one can be provided at a given time." );
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool IsOptionProvided(
            OptionStore option,
            Dictionary<string, object> parsedOptions )
        {
            var longName = option.OptionsAttribute.LongName;
            var shortName = option.OptionsAttribute.ShortName.ToString();
            var aliases = option.OptionsAttribute.Aliases;

            //Logger.Debug( $"Checking if option is provided:" );
            //Logger.Debug( $"Long name: {longName}, present: {parsedOptions.ContainsKey( longName )}" );
            //Logger.Debug( $"Short name: {shortName}, present: {parsedOptions.ContainsKey( shortName )}" );
            foreach( var alias in aliases )
            {
                Logger.Debug( $"Alias: {alias}, present: {parsedOptions.ContainsKey( alias )}" );
            }

            return parsedOptions.ContainsKey( longName )
                   || parsedOptions.ContainsKey( shortName )
                   || aliases.Any( alias => parsedOptions.ContainsKey( alias ) );
        }

        /// <summary>
        /// Parses multi-word values until another option or the end of args is found.
        /// </summary>
        /// <param name="args">The original args passed to <see cref="NaturalLanguageParsing"/>.</param>
        /// <param name="index">The current index in the args array.</param>
        /// <param name="skipDelimiter">Flag to denote how many tokens to skip/move ahead to</param>
        /// <returns>The parsed value as a string.</returns>
        private string ParseMultiWordValue(
            string[] args,
            ref int index,
            bool skipDelimiter = true )
        {
            var valueBuilder = new List<string>();
            index = skipDelimiter ? index + 2 : index + 1; //skip option name and delimiter

            //continue collecting words until we hit the next 'is' or end of array
            while( index < args.Length )
            {
                //stop if we find the next option's 'is' delimiter
                if( index + 1 < args.Length && args[index + 1].ToLowerInvariant() == ParsingKeyword.Is.ToString().ToLowerInvariant() )
                {
                    break;
                }
                valueBuilder.Add( args[index] );
                index++;
            }

            index--; //move back one position since we moved 1 too far
            return string.Join( " ", valueBuilder );
        }


        /// <summary>
        /// Converts a value to the expected property type, handling special cases like <see langword="bool"/>
        /// as well as numeric conversions.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="optionName">Name of the options for which the processing is being done.</param>
        /// <returns>The converted value.</returns>
        private object ConvertToOptionType( object value, Type targetType, string optionName )
        {
            var valueStr = value.ToString()?.Trim( '"' ) ?? string.Empty;
            if( valueStr.Length == 0 )
            {
                throw new InvalidValueException( $"Missing value for the required parameter '{optionName}', please " +
                    "check that you have values for all the required parameters and try again." );
            }

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
                if( decimal.TryParse( valueStr, out var decimalResult ) )
                {
                    return (int)Math.Floor( decimalResult );
                }

                var errorMessage = $"Invalid integer value: {valueStr}";
                throw new InvalidValueException( errorMessage );
            }

            return Convert.ChangeType( valueStr, targetType );
        }
    }
}
