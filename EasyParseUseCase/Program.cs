using EasyParser.Core;

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


     */
    public static class Program
    {
        public static void Main( string[] args )
        {
            var options = OptionsDeserializer.DeserializeOptions( typeof( ParseOptions ) );
            foreach( var option in options )
            {
                Console.WriteLine( $"{option.ToString()} \n" );
            }

            var optionsWithVerb = VerbDeserializer.DeserializeVerbs();
            foreach( var option in optionsWithVerb )
            {
                Console.WriteLine( $"{option.ToString()} \n" );
            }
        }
    }
}