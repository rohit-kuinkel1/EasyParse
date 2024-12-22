namespace EasyParser.Parsing
{
    /// <summary>
    /// This class in contrary to <see cref="StandardLanguageParsing"/>.
    /// <see cref="NaturalLanguageParsing"/> aims to parse the args provided to <see cref="EasyParse"/> 
    /// where the args are passed in a natural flow of language
    /// for instance: 
    /// addFile where name is Text123.txt filePath is D:/git/Tools/ smallerThan is 5KB
    /// a where where n is Text123.txt f is D:/git/Tools/ s is 5KB
    /// </summary>
    internal class NaturalLanguageParsing : IParsing
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        public ParsingResult<T> Parse<T>( string[] args ) where T : class, new()
        {
            return default;
        }
    }
}
