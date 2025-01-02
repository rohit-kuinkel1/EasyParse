using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyParser.Core
{
    /// <summary>
    /// <see cref="ParsingResultStore"/> represents a store or collection of all the <see cref="ParsingResult{T}"/>.
    /// It contains properties like <see cref="_parsedInstances"/> which holds all the <see cref="ParsingResult{T}"/>
    /// </summary>
    public class ParsingResultStore
    {
        /// <summary>
        /// Store for all the <see cref="ParsingResult{T}"/> instances
        /// </summary>
        private readonly ICollection<object> _parsedInstances;

        /// <summary>
        /// Default constructor for <see cref="ParsingResultStore"/>.
        /// Instantiates <see cref="_parsedInstances"/> with a new <see cref="List{T}"/> where 
        /// <typeparamref name="T"/> is of type <see langword="object"/>.
        /// </summary>
        internal ParsingResultStore()
        {
            _parsedInstances = new List<object>();
        }

        /// <summary>
        /// <see cref="AddResult{T}(ParsingResult{T})"/> adds a <see cref="ParsingResult{T}"/> passed as param
        /// to the <see cref="_parsedInstances"/> using the <see cref="List{T}.Add(T)"/> method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        internal void AddResult<T>( ParsingResult<T> result ) where T : class, new()
        {
            _parsedInstances.Add( result );
        }

        /// <summary>
        /// <see cref="GetResults{T}"/> gets a specific <see cref="ParsingResult{T}"/>
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<ParsingResult<T>> GetResults<T>() where T : class, new()
        {
            return _parsedInstances.OfType<ParsingResult<T>>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach( var parsedInstance in _parsedInstances )
            {
                stringBuilder.AppendLine( parsedInstance.ToString() );
            }
            return stringBuilder.ToString();
        }
    }


    /// <summary>
    /// <see cref="ParsingResult{T}"/> represents the result of a parsing operation, 
    /// including success status with <see cref="Success"/>, error message with <see cref="ErrorMessage"/>,
    /// and the parsed instance which is stored in <see cref="ParsedInstance"/>.
    /// </summary>
    public class ParsingResult<T> where T : class, new()
    {
        /// <summary>
        /// Get auto property for a value that indicates whether the parsing was successful or not
        /// </summary>
        public bool Success { get; }

        /// <summary>
        ///  Get auto property for the error message. 
        ///  Has a value if the parsing was not successful; otherwise, null/string.Empty.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Get auto property for the instance of the class that was parsed. 
        /// This will be populated if parsing is successful i.e <see cref="Success"/> was set to <see langword="true"/>.
        /// </summary>
        public T? ParsedInstance { get; }

        /// <summary>
        /// Default Parameterized constructor for <see cref="ParsingResult{T}"/>.
        /// Initializes a new instance of the <see cref="ParsingResult{T}"/> class with the provided params.
        /// </summary>
        /// <param name="success">Indicates whether the parsing was successful.</param>
        /// <param name="errorMessage">The error message associated with the parsing result, if any.</param>
        /// <param name="parsedInstance">The instance of the class that was parsed, if applicable.</param>
        public ParsingResult( bool success, string? errorMessage, T? parsedInstance )
        {
            Success = success;
            ErrorMessage = errorMessage;
            //if the where T : class is omitted in class definition, then the compiler complains when null/default is set, but i think that it makes sense to include the where clause in the class definition 
            //since T will be user defined type anyways (for now atleast)
            ParsedInstance = parsedInstance ?? default;
        }

        /// <summary>
        /// Returns a string representation of the parsed instance <see cref="ParsedInstance"/>,
        /// including all the property names and their values for this particular instance <see cref="ParsedInstance"/>.
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A string that lists all the properties and their values for the parsed instance.</returns>
        public override string ToString()
        {
            try
            {
                _ = EasyParser.Utility.Utility.NotNullValidation(
                    obj: ParsedInstance,
                    throwIfNull: true,
                    $"The property {nameof( ParsedInstance )} was null or empty when it was not e" );

                var instanceType = ParsedInstance!.GetType();

                //dont think its a good idea to get private attributes so sticking to public ones
                var properties = instanceType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

                var stringBuilder = new StringBuilder();
                _ = stringBuilder.AppendLine( $"{instanceType.Name} Properties:" );

                foreach( var property in properties )
                {
                    var value = property.GetValue( ParsedInstance ) ?? "null";
                    _ = stringBuilder.AppendLine( $"{property.Name}({property.PropertyType.Name}): {value}" );
                }

                return stringBuilder.ToString();
            }
            catch( Exception ex )
            {
                Logger.Error( $"An unexpected error occured whilst trying to use ParsingResult.ToString()\n{ex.Message}" );
                return $"ERROR: ParsingResult.ToString(): {ex.Message}";
            }
        }
    }
}