namespace EasyParser.Parsing
{
    /// <summary>
    /// <see cref="IParsing"/> is the contract for all the parsers to follow.
    /// </summary>
    public interface IParsing
    {
        /// <summary>
        /// parses the provided <paramref name="args"/> to be used and stored internally for later use.
        /// </summary>
        /// <param name="args"></param>
        ParsingResult<T> Parse<T>( string[] args ) where T : class, new();
    }
}
