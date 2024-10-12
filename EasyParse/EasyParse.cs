using System;
using System.Collections.Generic;
using System.Linq;
using EasyParser.Enums;
using EasyParser.Parsing;
using EasyParser.Utility;

namespace EasyParser
{
    /*
     parsing of args and how easyparse will accept args
    EasyParse, for now at least, will focus on pre build command line args
    The args will be parsed in SQL style or human readable format.
    For instance: 
    addFile where Name is TextFile.txt and FilePath is C:/git/Demo and IsSmallerThan is 5KB and IsInFormat is UTF-8 with BOM
    this will be parsed as addFile being the main verb with its options starting from where, meaning Name,FilePath,IsSmallerThan and IsInFormat are options
    with respective values of TextFile.txt, C:/git/Demo, 5KB and UTF-8 with BOM

    for this to work the solution where this library will be used must have this following rough structure.
    [Verb('a', "addFile", Required=false, HelpText="add, Aliases="af")]
    class EasyParse //name can be anything of the class,just this class must have the attribute attached to it
    {
        //option long names are case insensitive, meaning NAME,nAmE or name all are treated the same to avoid nuisance
        [Option('n',"name",Required=true,Description="Name of the file to add", HelpText= "normal file name structure in string format,no need of quotes even for spaces in string")]
        public string FileName{get;set;}

        [Option('p',"FilePath",Required=true,Description="Path where the file is to be added, HelpText= "normal path structure in string format,no need of quotes even for spaces in string")]
        public string FileName{get;set;}
        
        ........

    }
     
    A simple "did you mean" feature will help a lot during parsing.
    One way to achieve this is to use a source generator to already gather the verbs and options during compile time 
    in a ~collection like Dictionary<string,List<string>> where key will be the verb since one verb can have n number of options
    and the value will be a collection of options for that specific verb

    Since we will already have generated this during compile time, runtime lookups for the algorithm we decide to go with will be extremely efficient, almost negligible cost
    and the suggestions will also be almost 100% accurate, making the life of the user easier
     */


    /// <summary>
    /// public entry point for the Parser.
    /// </summary>
    public class EasyParse
    {
        #region privateFields

        /// <summary>
        /// abstraction of the parsing type to parse the args
        /// </summary>
        private IParsing? _parsing;

        /// <summary>
        /// HashSet of reserved keywords during arg parsing.
        /// Contains all the values of the <see cref="EasyParser.Enums.ParsingKeyword"/> in string format.
        /// </summary>
        private static readonly HashSet<string> Keywords = new HashSet<string>( Enum.GetNames( typeof( ParsingKeyword ) ).Select( k => k.ToLowerInvariant() ) );

        #endregion

        /// <summary>
        /// Default Constructor for <see cref="EasyParse"/>
        /// </summary>
        public EasyParse( LogLevel logLevel = LogLevel.Info )
        {
            Logger.Initialize( logLevel );
        }

        /// <summary>
        /// Parses the arguments provided to an instance of <see cref="EasyParse"/> and delegates the processing to the respective class.
        /// If the natural language syntax was used, 'where' keyword must be used at index 1 to denote that natural language has been used.
        /// If the conventional syntax was used, there is no need to use the 'where' keyword.
        /// <para>
        /// Although <paramref name="type"/> is optional, it is highly recommended to facilitate Parse with type so that the cost of reflection is as low as possible during runtime.
        /// </para>
        /// </summary>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <returns>True if the parsing was successful, False if the parsing was not successful.</returns>
        /// <exception cref="NullException"> When the provided <paramref name="args"/> was null.</exception>
        /// <exception cref="BadFormatException"> When the provided <paramref name="args"/> was badly formatted.</exception>
        /// <exception cref="IllegalOperation"> When type mismatch occurs or when static/abstract class is provided as type for instance.</exception>
        /// <exception cref="Exception"> For general unforseen exceptions.</exception>
        public bool Parse( string[] args, Type? type )
        {
            try
            {
                _ = Utility.Utility.NotNullValidation( args, true );

                // determine if we are using Natural Language or Standard Language parsing
                var isStringAtIndex1EqualToWhereKeyword = args.Length > 1 && string.Equals( args[1], ParsingKeyword.Where.ToString(), StringComparison.OrdinalIgnoreCase );
                var containsKeywords = args.Any( arg => Keywords.Equals( arg.ToLowerInvariant() ) );

                // using ternary operator here will turn ugly real fast
                if( isStringAtIndex1EqualToWhereKeyword && containsKeywords )
                {
                    _parsing = new NaturalLanguageParsing();
                }
                else if( !containsKeywords )
                {
                    _parsing = new StandardLanguageParsing();
                }
                else
                {
                    throw new BadFormatException( "Invalid structure for input args. Reserved keywords were detected for standard parsing. " +
                    "Please refrain from mixing natural language and standard language format and try again." );
                }

                return _parsing.Parse( args, type );
            }
            catch( Exception ex ) when( ex is NullException )
            {
                Console.WriteLine( ex.Message );
                return false;
            }
            catch( Exception ex ) //include StackTrace for general errors including BadFormatException
            {
                Console.WriteLine( ex.Message );
                Console.WriteLine( ex.StackTrace );
                return false;
            }
        }
    }
}
