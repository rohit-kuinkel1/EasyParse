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

        public abstract ParsingResult<T> Parse<T>( string[] args ) where T : class, new();

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
        /// retrieves the properties marked with the <see cref="OptionsAttribute"/> in the type provided to Parse.
        /// </summary>
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
        /// Validates the mutual agreement between options
        /// </summary>
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
        /// Converts a value to the expected property type
        /// </summary>
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
    }
}