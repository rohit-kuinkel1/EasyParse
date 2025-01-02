using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParser.Core;
using EasyParser.Enums;

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
    internal sealed class NaturalLanguageParsing : Parsing
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        public override ParsingResult<T> ParseOne<T>( string[] args )
        {
            Logger.BackTrace( $"Entering {nameof( NaturalLanguageParsing )}.{nameof( ParseOne )} with args " +
                $"with Len:{args.Length} and type T as {typeof( T ).FullName}" );

            _ = Utility.Utility.NotNullValidation( args );

            _verbStore = new Verb( typeof( T ), null );

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
                if( Utility.Utility.NotNullValidation( optionsAttribute, throwIfNull: false ) )
                {
                    var optionStore = new Option( property, optionsAttribute );
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
        /// Helper method to parse the actual input <paramref name="args"/>
        /// Examples:
        /// addFile where name is Text123.txt filePath is D:/git/Tools/ smallerThan is 5KB (ok)
        /// addFile where name is Text123.txt filePath is D:/git/Tools/ isEdible is false count is 10 (ok)
        /// addFile where name is Text123.txt filePath is D:/git/Tools/ isEdible is null count is noCount (wrong because isEdible is bool cant have null, count expects int cant have string)
        /// </summary>
        /// <param name="args"></param>
        /// <param name="verbStore"></param>
        /// <param name="instance"></param>
        /// <returns>A bool that indicates whether the parsing was successful or not</returns>
        private bool ParseOptions( string[] args, Verb verbStore, object instance )
        {
            Logger.BackTrace( $"Entering NaturalLanguageParsing.ParseOptions with args Len:{args.Length}" );

            _ = Utility.Utility.NotNullValidation( args );
            _ = Utility.Utility.NotNullValidation( verbStore );
            _ = Utility.Utility.NotNullValidation( instance );

            var parsedOptions = new Dictionary<string, object>();
            var whereIndex = args.Select( ( arg, index ) => new { arg, index } )
                .FirstOrDefault( x => string.Equals( x.arg.ToLowerInvariant(), ParsingKeyword.Where.ToString().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase ) )?.index ?? -1;

            if( whereIndex == -1 )
            {
                Logger.Critical( $"Parsing in a natural flow requires the '{ParsingKeyword.Where}' keyword" );
                return false;
            }

            for( var i = whereIndex + 1; i < args.Length; )
            {
                if( i + 2 < args.Length && args[i + 1].ToLowerInvariant() == ParsingKeyword.Is.ToString().ToLowerInvariant() )
                {
                    var optionName = args[i];
                    var value = ParseMultiWordValue( args, ref i );
                    parsedOptions[optionName] = value;
                }

                i++;
            }

            var isProcessingSuccessful = ProcessParsedOptions( verbStore, instance, parsedOptions );
            return isProcessingSuccessful;
        }

        private string ParseMultiWordValue( string[] args, ref int index )
        {
            var valueBuilder = new List<string>();
            if( string.Equals( args[index + 1], "is", StringComparison.OrdinalIgnoreCase ) )
            {
                index += 2; //skip the current option name and "is" keyword
            }

            //continue collecting words until we find the next "is" keyword or end of array
            //here index has already skipped the option and the is keyword
            while( index < args.Length )
            {
                if( index + 1 < args.Length &&
                    string.Equals( args[index + 1], ParsingKeyword.Is.ToString(), StringComparison.OrdinalIgnoreCase ) )
                {
                    //move back one more position to point to the next option name for the next iteration
                    index--;
                    break;
                }

                valueBuilder.Add( args[index] );
                index++;
            }

            var resultString = string.Join( " ", valueBuilder );
            return resultString;
        }

        private bool ProcessParsedOptions( Verb verbStore, object instance, Dictionary<string, object> parsedOptions )
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
    }
}