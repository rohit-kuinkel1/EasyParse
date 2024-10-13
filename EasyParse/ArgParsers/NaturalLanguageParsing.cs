namespace EasyParser.Parsing
{
    /// <summary>
    /// This class in contrary to <see cref="StandardLanguageParsing"/>, aims to parse the args provided to EasyParser where the args are passed in a natural flow
    /// for instance: 
    /// addFile where name is Text123.txt and filePath is D:/git/Tools/ and smallerThan is 5KB
    /// af where where n is Text123.txt and f is D:/git/Tools/ and s is 5KB
    /// </summary>
    internal class NaturalLanguageParsing : IParsing
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        /// <param name="type"></param>
        public ParsingResult<T> Parse<T>( string[] args ) where T : new()
        {
            return null;
        }
    }
}
