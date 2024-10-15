namespace EasyParser.Parsing
{
    /// <summary>
    /// contract for all the parsers to follow
    /// </summary>
    public interface IParsing
    {
        /// <summary>
        /// Parses the provided <paramref name="args"/> to be used and stored internally for later use.
        /// Passing type will significantly reduce the reflection overhead since we won't need to reflect many classes.
        /// </summary>
        /// <param name="args"></param>
        ParsingResult<T> Parse<T>( string[] args ) where T : new();
    }
}
