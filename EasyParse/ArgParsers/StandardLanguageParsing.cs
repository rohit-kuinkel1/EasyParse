namespace EasyParse.Parsing
{
    /// <summary>
    /// This class in contrary to <see cref="NaturalLanguageParsing"/>, aims to parse the args provided to EasyParser where the args are passed in the standard flow
    /// for instance: 
    /// addFile --name Text123.txt --filePath D:/git/Tools/ --smallerThan 5KB ......
    /// af -n Text123.txt -f D:/git/Tools/ -s 5KB ......
    /// </summary>
    internal class StandardLanguageParsing : IParsing
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="args"></param>
        public void Parse( string[] args )
        {
        }
    }
}
