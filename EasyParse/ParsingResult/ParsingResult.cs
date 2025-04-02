using System;
using System.Reflection;
using System.Text;
using EasyParse.Utility;

namespace EasyParse.Core
{
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
        public bool Success { get; } = false;

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
            Logger.BackTrace( $"Entering {nameof( ParsingResult<T> )}.{nameof( ToString )}" );
            var stringBuilder = new StringBuilder();
            try
            {
                if( !Success )
                {
                    _ = stringBuilder.AppendLine( $"Success: False, ErrorMessage: {ErrorMessage}, ParsedInstance: null" );
                    return stringBuilder.ToString();
                }

                _ = EasyParse.Utility.Utility.NotNullValidation(
                    obj: ParsedInstance,
                    throwIfNull: true,
                    $"The property {nameof( ParsedInstance )} was null or empty when it was not expected to be" );

                //If ParsedInstance was null, we would never reach here cause of throwIfNull: true so its safe to use !
                var instanceType = ParsedInstance!.GetType();

                var properties = instanceType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

                _ = stringBuilder.AppendLine( $"Properties for parsed instance {instanceType.Name}:" );
                foreach( var property in properties )
                {
                    var value = property.GetValue( ParsedInstance ) ?? "null";
                    _ = stringBuilder.AppendLine( $"{property.Name}({property.PropertyType.Name}): {value}" );
                }

                return stringBuilder.ToString();
            }
            catch( Exception ex ) when( ex is not NullException )
            {
                Logger.Error( $"An unexpected error occured whilst trying to use ParsingResult.ToString()\n{ex.Message}" );
                return $"ERROR: ParsingResult.ToString(): {ex.Message}";
            }
        }
    }
}
