using System;
using System.Collections.Generic;
using System.Reflection;
using EasyParser.Core;
using EasyParser.Utility;

namespace EasyParser.Parsing
{
    /// <summary>
    /// <see cref="StandardLanguageParsing"/>is contrary to <see cref="NaturalLanguageParsing"/>
    /// and aims to parse the args provided to EasyParser where the args are passed in the standard flow
    /// for instance: 
    /// addFile --name Text123.txt --filePath D:/git/Tools/ --smallerThan 5KB ......
    /// a -n Text123.txt -f D:/git/Tools/ -s 5KB ......
    /// </summary>
    internal sealed class StandardLanguageParsing : BaseParsing
    {
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
        ///// <param name="longNamePrefix"></param>
        ///// <param name="shortNamePrefix"></param>
        public StandardLanguageParsing( /*string longNamePrefix = "--", char shortNamePrefix = '-'*/ )
        {
            _longNamePrefix = "--";
            _shortNamePrefix = '-';
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        public override ParsingResult<T> ParseOne<T>( string[] args ) // where T : class, new() //compiler will complain if you uncomment, here for readability
        {
            Logger.BackTrace( $"Parsing type {typeof( T ).FullName}" );

            try
            {
                _ = Utility.Utility.NotNullValidation( args );

                _verbStore = new Verb( typeof( T ), null, new List<Option>() );

                if( typeof( T ).IsDefined( typeof( VerbAttribute ), inherit: false ) )
                {
                    _verbStore.VerbAttribute = Attribute.GetCustomAttribute( typeof( T ), typeof( VerbAttribute ) ) as VerbAttribute;
                }
                else
                {
                    throw new IllegalOperationException( $"Type/Class {typeof( T ).FullName} was not marked with the decorator VerbAttribute and hence cannot be used to parse args." );
                }

                //example: parser.Parse<ParseVerbs>( args ); instantiate an object of type ParseVerbs passed by the user
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
                    var parsingResultFailure = new ParsingResult<T>( false, "Parsing Status: ERROR", default! );
                    Logger.BackTrace( $"Could not parsed the provided options, returning: {parsingResultFailure}" );
                    return parsingResultFailure;
                }

                var parsingResultSuccess = new ParsingResult<T>( true, null, instance );
                Logger.BackTrace( $"Parsed the provided options successfully, returning: {parsingResultSuccess}" );
                Logger.BackTrace( _verbStore.ToString() );
                return parsingResultSuccess;
            }
            catch( Exception ex )
            {
                return new ParsingResult<T>( false, ex.Message, default! );
            }
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
        /// <exception cref="EasyParser.Utility.NullException"></exception>
        protected override bool ParseOptions( string[] args, Verb verbStore, object instance )
        {
            Logger.BackTrace( $"Entering StandardLanguageParsing.ParseOptions with args Len:{args.Length}" );

            _ = Utility.Utility.NotNullValidation( args );
            _ = Utility.Utility.NotNullValidation( verbStore );
            _ = Utility.Utility.NotNullValidation( instance );

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

            var isProcessingSuccessful = ProcessParsedOptions( verbStore, instance, parsedOptions );
            return isProcessingSuccessful;
        }

        /// <summary>
        /// <see cref="ParseMultiWordValue(string[], ref int, string, char)"/> parses multi-worded value from by combining consecutive words until it encounters 
        /// another option or reaches the end of arguments. It considers words as part of the value until 
        /// it finds another argument prefixed with the specified long or short name prefixes ("-" or "--").
        /// </summary>
        /// <remarks>
        /// This method is designed for handling command line arguments where values might contain multiple words,
        /// such as "program --description This is a long description" or "program -n John Doe Smith".
        /// The method will capture all words following the option until it either:
        /// 1. Reaches another option (prefixed with longNamePrefix or shortNamePrefix)
        /// 2. Reaches the end of the arguments array
        /// </remarks>
        /// <param name="args">The complete array of command line arguments being parsed</param>
        /// <param name="index">Current position in the args array. Will be modified to point to the last consumed argument</param>
        /// <param name="longNamePrefix">The prefix used for long option names (typically "--")</param>
        /// <param name="shortNamePrefix">The prefix used for short option names (typically '-')</param>
        /// <returns>A string containing all the words concatenated with spaces</returns>
        /// <example>
        /// For input: ["--description", "This", "is", "text", "--next-option"]
        /// Returns: "This is text"
        /// </example>
        private static string ParseMultiWordValue( string[] args, ref int index, string longNamePrefix, char shortNamePrefix )
        {
            Logger.BackTrace( $"Entering {nameof( ParseMultiWordValue )}" );
            var valueBuilder = new List<string>();
            index++;

            while( index < args.Length && !args[index].StartsWith( longNamePrefix ) && !args[index].StartsWith( shortNamePrefix ) )
            {
                valueBuilder.Add( args[index] );
                index++;
            }

            index--;
            return string.Join( " ", valueBuilder );
        }
    }
}