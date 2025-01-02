using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyParser.Core;
using EasyParser.Enums;
using EasyParser.Utility;

namespace EasyParser.Parsing
{
    /// <summary>
    /// Base class containing common functionality for parsing implementations
    /// </summary>
    //if the base class implements the interface, then all the children will/should implement it automatically, which follows LSP
    internal abstract class Parsing : IParsing
    {
        /// <summary>
        /// Stores all the propertyInfos for the classes marked with <see cref="VerbAttribute"/> regardless of the binding flags.
        /// </summary>
        protected PropertyInfo[]? _allPropertyInfosFromType;

        /// <summary>
        /// Container to store the <see cref="VerbAttribute"/> and its related <see cref="OptionsAttribute"/>.
        /// </summary>
        protected Verb? _verbStore;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract ParsingResult<T> ParseOne<T>( string[] args ) where T : class, new();

        /// <summary>
        /// If the log level permits and some non-public properties are marked with <see cref="OptionsAttribute"/>, 
        /// then let the user know about it using <see cref="Logger.Debug(string)"/>
        /// </summary>
        protected void LogPotentialNonPublicPropertyMarkedWithOptionsAttribute()
        {
            Logger.BackTrace( $"Entering {GetType().Name}.LogPotentialNonPublicPropertyMarkedWithOptionsAttribute()" );
            var nonPublicPropertiesWithOptionsAttribute = GetPropertyBy( BindingFlags.NonPublic | BindingFlags.Instance );
            if( nonPublicPropertiesWithOptionsAttribute.Length != 0 )
            {
                foreach( var property in nonPublicPropertiesWithOptionsAttribute )
                {
                    Logger.Debug( $"Property '{property.Name}' is marked with OptionsAttribute but is not set to public." +
                        $"Knowingly avoiding non-public property {property.Name}..." );
                }
            }
        }

        /// <summary>
        /// retrieves the properties marked with the <see cref="OptionsAttribute"/> in the type provided to <see cref="ParseOne(string[])"/>.
        /// Param <paramref name="bindingFlags"/> is used to retrieve the properties with specific access specifiers.
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns> Array of <see cref="PropertyInfo"/></returns>
        /// <exception cref="Utility.NullException">Thrown when the <see cref="_allPropertyInfosFromType"/> has no properties in it.</exception>
        protected PropertyInfo[] GetPropertyBy( BindingFlags bindingFlags )
        {
            Logger.BackTrace( $"Entering {GetType().Name}.GetPropertyBy(BindingFlags) with bindingFlags {bindingFlags}" );

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
        /// One size fits all
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        protected bool ValidateCommonAttributes(
            ICollection<Option> options,
            Option currentOption,
            Dictionary<string, object> parsedOptions )
        {
            return
                ValidateMutualRelationships( options, currentOption, parsedOptions )
                && ValidateSettings( options, currentOption, parsedOptions );
        }

        /// <summary>
        /// Validates the mutual agreement <see cref="MutualAttribute"/> between <see cref="OptionsAttribute"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        protected bool ValidateMutualRelationships(
            ICollection<Option> options,
            Option currentOption,
            Dictionary<string, object> parsedOptions )
        {
            foreach( var mutualAttr in currentOption.MutualAttributes )
            {
                foreach( var relatedEntity in mutualAttr.RelatedEntities )
                {
                    var relatedOption = options.FirstOrDefault( o => o.Property.Name == relatedEntity );

                    if( relatedOption != null )
                    {
                        var isCurrentOptionProvided = IsOptionProvided( currentOption, parsedOptions );
                        var isRelatedOptionProvided = IsOptionProvided( relatedOption, parsedOptions );

                        if( mutualAttr.RelationshipType == MutualType.Inclusive && ( isCurrentOptionProvided ^ isRelatedOptionProvided ) )
                        {
                            Logger.Critical( $"Options {nameof( currentOption.OptionsAttribute.LongName )}:'{currentOption.OptionsAttribute.LongName}' " +
                                $"and {nameof( relatedOption.OptionsAttribute.LongName )}:'{relatedOption.OptionsAttribute.LongName}' " +
                                $"were defined to be mutually inclusive to each other. Both must be provided at the same time." );
                            return false;
                        }
                        else if( mutualAttr.RelationshipType == MutualType.Exclusive && ( isCurrentOptionProvided && isRelatedOptionProvided ) )
                        {
                            Logger.Critical( $"Options '{currentOption.OptionsAttribute.LongName}' " +
                                $"and '{relatedOption.OptionsAttribute.LongName}' " +
                                $"cannot be used together because they were marked to be mutually exclusive." );
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if an option is provided in the parsed options
        /// </summary>
        protected bool IsOptionProvided( Option option, Dictionary<string, object> parsedOptions )
        {
            var longName = option.OptionsAttribute.LongName;
            var shortName = option.OptionsAttribute.ShortName.ToString();
            var aliases = option.OptionsAttribute.Aliases;

            return parsedOptions.ContainsKey( longName )
                   || parsedOptions.ContainsKey( shortName )
                   || aliases.Any( alias => parsedOptions.ContainsKey( alias ) );
        }

        /// <summary>
        /// <see cref="ConvertToOptionType(object, Type, string)"/> converts a <paramref name="value"/> to the expected property type <paramref name="targetType"/>.
        /// </summary>
        /// <remarks>
        /// Used mostly when the user provides values for options in unconventional ways. 
        /// For example: 
        /// <para>
        /// --isTrue "tRuE" or --isTrue TRUE or --isTrue TrUe
        /// </para>
        /// <para>
        /// --Count "20"
        /// </para>
        /// </remarks>
        protected object ConvertToOptionType( object value, Type targetType, string optionName )
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
                throw new InvalidValueException( $"Invalid boolean value: {valueStr}" );
            }

            if( targetType == typeof( int ) )
            {
                if( decimal.TryParse( valueStr, out var decimalResult ) )
                {
                    return (int)Math.Floor( decimalResult );
                }
                throw new InvalidValueException( $"Invalid integer value: {valueStr}" );
            }

            return Convert.ChangeType( valueStr, targetType );
        }

        /// <summary>
        /// Validates a value against the settings defined in <see cref="SettingsAttribute"/>.
        /// This includes color settings, numeric range validation, and regex pattern matching...
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        protected bool ValidateSettings(
            ICollection<Option> options,
            Option currentOption,
            Dictionary<string, object> parsedOptions )
        {
            var settings = currentOption.SettingsAttribute;
            if( settings == null )
            {
                return true;
            }

            //get the actual value from parsed options
            var value = parsedOptions.FirstOrDefault( p =>
                p.Key == currentOption.OptionsAttribute.LongName ||
                p.Key == currentOption.OptionsAttribute.ShortName.ToString() ||
                currentOption.OptionsAttribute.Aliases.Contains( p.Key ) ).Value;

            if( value == null )
            {
                return true;
            }

            var stringValue = value.ToString()?.Trim( '"' ) ?? string.Empty;
            var propertyType = currentOption.Property.PropertyType;
            var optionName = currentOption.OptionsAttribute.LongName;

            //validate numeric range if applicable / possible
            if( propertyType == typeof( int ) && ( settings.MinValue != SettingsAttribute.DefaultNotProvidedMinMax && settings.MaxValue != SettingsAttribute.DefaultNotProvidedMinMax ) )
            {
                if( !int.TryParse( stringValue, out int numericValue ) )
                {
                    Logger.Critical( $"Value '{stringValue}' for option '{optionName}' must be a valid integer." );
                    return false;
                }
                if( numericValue < settings.MinValue )
                {
                    Logger.Critical(
                        $"Value {numericValue} for option '{optionName}' is below the minimum allowed value of {settings.MinValue}." );
                    return false;
                }

                if(  numericValue > settings.MaxValue )
                {
                    Logger.Critical(
                        $"Value {numericValue} for option '{optionName}' exceeds the maximum allowed value of {settings.MaxValue}." );
                    return false;
                }
            }

            //validate regex pattern if a regex pattern was specified for this property
            if( settings.CompiledRegex == null && !string.IsNullOrEmpty( settings.RegexPattern ) )          
            {
                settings.CompiledRegex = new Regex( settings.RegexPattern, RegexOptions.Compiled );
                if( !settings.CompiledRegex.IsMatch( stringValue ) )
                {
                    var errorMessage = string.IsNullOrEmpty( settings.RegexOnFailureMessage )
                        ? $"Value '{stringValue}' for option '{optionName}' does not match the required pattern."
                        : settings.RegexOnFailureMessage;

                    Logger.Critical( errorMessage );
                    return false;
                }
            }

            return true;
        }
    }
}