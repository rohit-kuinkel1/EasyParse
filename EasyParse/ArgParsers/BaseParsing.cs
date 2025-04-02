using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.Core;
using EasyParse.Enums;
using EasyParse.Utility;

namespace EasyParse.Parsing
{
    /// <summary>
    /// Base class containing common functionality for parsing implementations
    /// </summary>
    //if the base class implements the interface, then all the children will/should implement it automatically, which follows LSP
    internal abstract class BaseParsing : IParsing
    {
        #region CommonClassProperty
        /// <summary>
        /// Stores all the propertyInfos for the classes marked with <see cref="VerbAttribute"/> regardless of the binding flags.
        /// </summary>
        protected PropertyInfo[]? _allPropertyInfosFromType;

        /// <summary>
        /// Container to store the <see cref="VerbAttribute"/> and its related <see cref="OptionsAttribute"/>.
        /// </summary>
        protected Verb? _verbStore;
        #endregion

        #region AbstractMethods
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract ParsingResult<T> ParseOne<T>( string[] args ) where T : class, new();

        /// <summary>
        /// Parses the options provided.
        /// Each child of <see cref="BaseParsing"/> will have its own implementations
        /// </summary>
        /// <param name="args"></param>
        /// <param name="verbStore"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected abstract bool ParseOptions( 
            string[] args, 
            Verb verbStore, 
            object instance 
        );
        #endregion

        #region PropertyManipulationOfInstance
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
                foreach( var propertyName in nonPublicPropertiesWithOptionsAttribute.Select( np => np.Name ) )
                {
                    Logger.Debug( $"Property '{propertyName}' is marked with OptionsAttribute but is not set to public." +
                        $" Knowingly avoiding non-public property {propertyName}..." );
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
        #endregion

        #region BaseProcessParsedOptions
        /// <summary>
        /// Processes the parsed options.
        /// Each child class will have their own implementations of <see cref="ParseOptions(string[], Verb, object)"/> but at the end
        /// they will follow this structure to process the parsed options since we will always end up with
        /// these 3 things: <paramref name="verbStore"/> storing the verb which inturn has a collection of <see cref="Option"/>
        /// in it, the parsed instance <see cref="ParsingResult{T}"/> and a dict of the parsedOptions, originally a 1D array
        /// of <see cref="string"/> but now mapped into a <see cref="Dictionary{TKey, TValue}"/> where the key represents
        /// the 1D array of string[] broken into individual tokens and the value represents the user passed value
        /// </summary>
        /// <param name="verbStore"></param>
        /// <param name="instance"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        protected static bool ProcessParsedOptions(
            Verb verbStore,
            object instance,
            IDictionary<string, object> parsedOptions
        )
        {
            foreach( var option in verbStore.Options )
            {
                try
                {
                    var optionAttr = option.OptionsAttribute;
                    var aliases = optionAttr.Aliases;

                    if( ( parsedOptions.TryGetValue( optionAttr.LongName, out var value )
                            || parsedOptions.TryGetValue( optionAttr.ShortName.ToString(), out value )
                            || aliases.Any( alias => parsedOptions.TryGetValue( alias, out value ) ) )
                        && Utility.Utility.NotNullValidation( value ) )
                    {
                        //validate all relations 
                        if( !ValidateCommonAttributes( verbStore.Options, option, parsedOptions ) )
                        {
                            return false;
                        }

                        var convertedValue = ConvertToOptionType( value, option.Property.PropertyType, option.OptionsAttribute.LongName );
                        option.Property.SetValue( instance, convertedValue );
                    }
                    else if( optionAttr.Required )
                    {
                        Logger.Critical( $"Option '{optionAttr.LongName}' was marked to be required, but was not provided." );
                        return false;
                    }
                }
                catch( Exception ex )
                {
                    Logger.Critical( ex.Message );
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// One size fits all
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        protected static bool ValidateCommonAttributes(
            ICollection<Option> options,
            Option currentOption,
            IDictionary<string, object> parsedOptions )
        {
            return
                ValidateMutualRelationships( options, currentOption, parsedOptions )
                && ValidateSettings( options, currentOption, parsedOptions );
        }
        #endregion

        #region ValidateMutualRelations
        /// <summary>
        /// Validates the mutual agreement <see cref="MutualAttribute"/> between <see cref="OptionsAttribute"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        protected static bool ValidateMutualRelationships(
            ICollection<Option> options,
            Option currentOption,
            IDictionary<string, object> parsedOptions )
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
        #endregion

        #region validateOptions
        /// <summary>
        /// Checks if an option is provided in the parsed options
        /// </summary>
        protected static bool IsOptionProvided(
            Option option,
            IDictionary<string, object> parsedOptions
        )
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
        protected static object ConvertToOptionType(
            object value,
            Type targetType,
            string optionName
        )
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
        #endregion

        #region ValidateSettings
        /// <summary>
        /// Validates a value against the settings defined in <see cref="SettingsAttribute"/>.
        /// This includes color settings, numeric range validation, and regex pattern matching...
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        protected static bool ValidateSettings(
            ICollection<Option> options,
            Option currentOption,
            IDictionary<string, object> parsedOptions
        )
        {
            var settings = currentOption.SettingsAttribute;
            if( settings == null )
            {
                return true;
            }

            // Get the actual value from parsed options
            var value = GetValueFromParsedOptions( currentOption, parsedOptions );
            if( value == null )
            {
                return true;
            }

            var stringValue = value.ToString()?.Trim( '"' ) ?? string.Empty;
            var propertyType = currentOption.Property.PropertyType;
            var optionName = currentOption.OptionsAttribute.LongName;

            if( !ValidateNumericRange( stringValue, settings, optionName ) )
            {
                return false;
            }

            if( !ValidateRegex( stringValue, settings, optionName ) )
            {
                return false;
            }

            if( !ValidateAllowedValues( value, settings, propertyType, stringValue, optionName ) )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Helper for <see cref="ValidateSettings(ICollection{Option}, Option, IDictionary{string, object})"/>
        /// </summary>
        /// <param name="currentOption"></param>
        /// <param name="parsedOptions"></param>
        /// <returns></returns>
        private static object GetValueFromParsedOptions(
            Option currentOption,
            IDictionary<string, object> parsedOptions
        )
        {
            return parsedOptions.FirstOrDefault( p =>
                p.Key == currentOption.OptionsAttribute.LongName ||
                p.Key == currentOption.OptionsAttribute.ShortName.ToString() ||
                currentOption.OptionsAttribute.Aliases.Contains( p.Key ) ).Value;
        }

        /// <summary>
        /// Helper for <see cref="ValidateSettings(ICollection{Option}, Option, IDictionary{string, object})"/>
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="settings"></param>
        /// <param name="optionName"></param>
        /// <returns></returns>
        private static bool ValidateNumericRange(
            string stringValue,
            SettingsAttribute settings,
            string optionName
        )
        {
            if( settings.MinValue == SettingsAttribute.DefaultNotProvidedMinMax || settings.MaxValue == SettingsAttribute.DefaultNotProvidedMinMax )
            {
                return true; // No validation if min or max are not provided
            }

            if( !int.TryParse( stringValue, out int numericValue ) )
            {
                Logger.Critical( $"Value '{stringValue}' for option '{optionName}' must be a valid integer." );
                return false;
            }

            if( numericValue < settings.MinValue )
            {
                Logger.Critical( $"Value {numericValue} for option '{optionName}' is below the minimum allowed value of {settings.MinValue}." );
                return false;
            }

            if( numericValue > settings.MaxValue )
            {
                Logger.Critical( $"Value {numericValue} for option '{optionName}' exceeds the maximum allowed value of {settings.MaxValue}." );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Helper for <see cref="ValidateSettings(ICollection{Option}, Option, IDictionary{string, object})"/>
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="settings"></param>
        /// <param name="optionName"></param>
        /// <returns></returns>
        private static bool ValidateRegex(
            string stringValue,
            SettingsAttribute settings,
            string optionName
        )
        {
            if( settings.CompiledRegex != null )
            {
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

        /// <summary>
        /// Helper for <see cref="ValidateSettings(ICollection{Option}, Option, IDictionary{string, object})"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <param name="propertyType"></param>
        /// <param name="stringValue"></param>
        /// <param name="optionName"></param>
        /// <returns></returns>
        private static bool ValidateAllowedValues(
            object value,
            SettingsAttribute settings,
            Type propertyType,
            string stringValue,
            string optionName
        )
        {
            if( settings.AllowedValues != null && settings.AllowedValues.Length > 0 )
            {
                var convertedValue = ConvertToOptionType( value, propertyType, optionName );

                bool isAllowed = settings.AllowedValues.Any( allowedValue =>
                    allowedValue?.GetType() == propertyType &&
                    allowedValue.Equals( convertedValue ) );

                if( !isAllowed )
                {
                    var allowedValuesStr = string.Join( ", ", settings.AllowedValues.Select( v => $"'{v}'" ) );
                    Logger.Critical( $"Value '{stringValue}' for option '{optionName}' is not one of the allowed values: {allowedValuesStr}" );
                    return false;
                }
            }

            return true;
        }
    }
    #endregion
}