using EasyParse.Core;

namespace Program
{

    /*
     end goal, one line call:
        EasyParser.Parse(args) where Parse returns a bool flag
        if the flag is false, the user can decide to do whatever they want
        But if EasyParser.Parse(args).FailOnNotParsed() is called, the parser should fail/throw an exception if args could not be parsed
     */
    public static class Program
    {
        public static void Main( string[] args)
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