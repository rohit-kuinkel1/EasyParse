using EasyParser;

namespace Program
{

    /*
     end goal, one line call:
        EasyParser.Parse(args) where Parse returns a bool flag
        if the flag is false, the user can decide to do whatever they want
        But if EasyParser.Parse(args).FailOnNotParsed() is called, the parser should fail/throw an exception if args could not be parsed

        In both the cases, when args is passed to the Parse function, it should take in the args and parse the args such that it checks the whole assembly for the args
        Example:
                addFile --name "TextBook.txt" --path "C:\git\" --isPdf false --sizeInKb 412
                should parse to a class with a verb attribute of addFile which has properties with OptionsAttributes attached to them
                where the options attributes longName corresponds to name,path,isPdf and sizeInKb.
                we should do the check with IsLowerInvariant() as to avoid annoying problems
                If no such class with verb attribute tag was found, the library gracefully closes,
                lets the user know that it didnt find a class defined with the verb attribute addFile which had
                all of these attributes


    12/10/2024:
    since we will have to use reflection to access the classes and its properties, it makes sense for the parse to take in one more arg, this will significantly reduce the 
    reflection needed during runtime since the library will only have to run the reflection on that one class, and since we are supporting natural language as well as the 
    standard way to parse, an object will be required.
    var parser = new EasyParse();
    parser.Parse( arg, typeof( ParseOptions ) )

    13/10/2024:
    initial prototype for Parse with SLP.cs is finished.
    Decided to go with a template style for the function call, this way it will remain clean while letting the users just pass the arg
    Also the response from Parse contains a Success flag so users can choose to react on the status of the arg parsing themselves with a very simple if/else block
    var result = easyParser.Parse<ClassWithVerb>(args);
    if(result.Success){}
    else{}
     */
    public static class Program
    {
        /// <summary>
        /// Entry Point of an application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main( string[] args )
        {
            //var args1 = new[] { "add", "--read", "Help.txt Ferrari Car", "--verbose", "True", "--stdin", "TRUE", "--count", "215" };
            //var args1 = new[] { "add", "where", "read", "is", "Help File Test.txt", "verbose", "is", "True", "stdin", "is", "TRUE", "count", "is", "215" };

            var videoArgs = new[] { "process", "--input", "vacation.mp4", "--quality", "1080p", "--duration", "300", "--force", "true" };
            //var videoArgs = new[] { "process", "where", "source", "is", "vacation.mp4", "resolution", "is", "1080p", "time", "is", "300", "force", "is", "true" };

            var dbArgs = new[] { "backup", "--host", "db.example.com", "--port", "5432", "--compress", "true", "--encrypt", "false" };
            //var dbArgs = new[] { "backup", "where", "server", "is", "db.example.com", "portnumber", "is", "5432", "compress", "is", "true", "encrypt", "is", "false" };

            var weatherArgs = new[] { "weather", "--location", "40.7128,-74.0060", "--interval", "15", "--units", "metric", "--offline", "true" };
            //var weatherArgs = new[] { "weather", "where", "coordinates", "is", "40.7128,-74.0060", "frequency", "is", "15", "system", "is", "metric", "offline", "is", "true" };

            var testArgs = new[]
            {
                "add", "--read", "Help File Test.txt", "--verbose", "True", "--stdin", "TRUE", "--count", "15",
                "&", 
                "process", "--input", "vacation.mp4", "--quality", "1080p", "--duration", "300", "--force", "true",
                "&",
                "backup", "--host", "db.example.com", "--port", "5432", "--compress", "true", "--encrypt", "false",
                "&",
                "weather", "--location", "40.7128,-74.0060", "--interval", "15", "--units", "metric", "--offline", "true"
            };

            //EasyParser.EasyParse.ExportDefaultConfig( exportWithMain: true );
            var parser = EasyParser.EasyParse.Initialize( 
                minLogLevel: EasyParser.LogLevel.BackTrace, 
                redirectLogsToFile: false, 
                logDirectory: @"C:\Users\kuike\Desktop\EasyParseLogs" 
            );

            parser.SetLoggerStatusEnabled( true );
            //var parsingResult = parser.Parse<ParseVerbs>( args1 );

            var parsingResult = parser.Parse<ParseVerbs, VideoProcessVerbs, DatabaseBackupVerbs, WeatherStationVerbs>( testArgs );

            if( parsingResult.Success )
            {
                var instances = parsingResult.ParsedInstances;
                foreach( var instance in instances )
                {
                    if( instance is ParseVerbs )
                    {
                    }
                    else if( instance is VideoProcessVerbs )
                    {
                    }
                    else if( instance is DatabaseBackupVerbs )
                    {
                    }
                    else if( instance is WeatherStationVerbs )
                    {
                    }

                    Console.WriteLine( instance.ToString() );
                }
            }
            else
            {
               // Console.WriteLine( "dsdasd" );
                //Console.WriteLine( parsingResult.Errors );
            }

        }
    }
}