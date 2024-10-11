using System;
using EasyParse.Parsing;
using EasyParse.Utility;

namespace Parser
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
        private IParsing? _parsing;

        /// <summary>
        /// Default Constructor for <see cref="EasyParse"/>
        /// </summary>
        public EasyParse()
        {
        }

        /// <summary>
        /// Parses the arguments provided to an instance of <see cref="EasyParse"/> and delegates the processing to the respective class.
        /// If the natural language syntax was used, 'where' keyword must be used at index 1 to denote that natural language has been used.
        /// If the conventional syntax was used, there is no need to use the 'where' keyword.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>True if the parsing was successful, False if an exception was thrown or parsing failed.</returns>
        /// <exception cref="NullException"> When the provided <paramref name="args"/> was null.</exception>
        /// <exception cref="Exception"> For general unforseen exceptions.</exception>
        public bool Parse( string[] args )
        {
            try
            {
                _ = Utility.NotNullValidation( args, true );

                _parsing = args.Length > 1 && string.Equals( args[1], "where", StringComparison.OrdinalIgnoreCase )
                    ? new NaturalLanguageParsing()
                    : new StandardLanguageParsing();

                _parsing?.Parse( args );
                return true;
            }
            catch( Exception ex ) when( ex is NullException )
            {
                Console.WriteLine( ex.Message );
                return false;
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.Message );
                Console.WriteLine( ex.StackTrace );
                return false;
            }
        }
    }
}
