using System;
using System.Reflection;
using System.Text;
using EasyParser;

/// <summary>
/// Represents the result of a parsing operation, including success status, error message, and the parsed instance.
/// </summary>
public class ParsingResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the parsing was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the error message if the parsing was not successful; otherwise, null.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the instance of the class that was parsed. This will be populated if parsing is successful.
    /// </summary>
    public T ParsedInstance { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingResult{T}"/> class.
    /// </summary>
    /// <param name="success">Indicates whether the parsing was successful.</param>
    /// <param name="errorMessage">The error message associated with the parsing result, if any.</param>
    /// <param name="parsedInstance">The instance of the class that was parsed, if applicable.</param>
    internal ParsingResult( bool success, string? errorMessage, T parsedInstance )
    {
        Success = success;
        ErrorMessage = errorMessage;
        ParsedInstance = parsedInstance; 
    }

    /// <summary>
    /// Returns a string representation of the parsed instance, including all the property names and their values.
    /// </summary>
    /// <returns>A string listing all the properties and their values for the parsed instance.</returns>
    public override string ToString()
    {
        try
        {
            if( ParsedInstance == null )
            {
                return "No instance was parsed.";
            }

            var instanceType = ParsedInstance.GetType();
            var properties = instanceType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine( $"{instanceType.Name} Properties:" );

            foreach( var property in properties )
            {
                var value = property.GetValue( ParsedInstance ) ?? "null";
                _ = stringBuilder.AppendLine( $"{property.Name}: {value}" );
            }

            return stringBuilder.ToString();
        }
        catch( Exception ex )
        {
            Logger.Error( $"An unexpected error occured whilst trying to use ParsingResult.ToString()...\n{ex.Message}" );
            return $"ERROR: ParsingResult.ToString(): {ex.Message}";
        }
    }
}
