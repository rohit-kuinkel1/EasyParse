using System.Linq;

namespace EasyParse.Core
{
    /// <summary>
    /// Provides functionality for generating Soundex codes for given input strings.
    /// The Soundex code is a phonetic algorithm used to represent how a string sounds.
    /// </summary>
    public static class SoundexProvider
    {
        /// <summary>
        /// Retrieves the Soundex code for a given input string.
        /// No cache is needed in our case of <see cref="EasyParser"/> because its a command line 
        /// parsing tool, meaning it will most likely just run once during cold start, which means we will never have cache anyways
        /// </summary>
        /// <param name="input">The string to generate the Soundex code for.</param>
        /// <returns>
        /// A 4-character string representing the Soundex code, padded with '0' if necessary.
        /// Returns an empty string if the input is null or empty.
        /// </returns>
        public static string GetCode( string input )
        {
            if( string.IsNullOrEmpty( input ) )
            {
                return string.Empty;
            }

            var code = GetDigit( input.ToUpper()[0] ).ToString();
            var previousCode = GetDigit( input.ToUpper()[0] );

            foreach( var c in input.ToUpper().Skip( 1 ).Where( char.IsLetter ) )
            {
                var currentCode = GetDigit( c );

                //only add non zero digits that differ from the previous code
                if( currentCode != '0' && currentCode != previousCode )
                {
                    code += currentCode;
                    if( code.Length == 4 ) break;
                }

                previousCode = currentCode;
            }

            //pad the code to 4 characters if necessary, ensures that the Soundex code always has a consistent length of 4 characters, which is the standard format for Soundex codes.
            return code.PadRight( 4, '0' );
        }

        /// <summary>
        /// Determines the Soundex digit for a given character based on the traditional Soundex rules.
        /// </summary>
        /// <param name="c">The character to be converted into a Soundex digit.</param>
        /// <returns>
        /// A character representing the Soundex digit (0-6), based on the input character.
        /// </returns>
        //( "Smith", "Smythe" )
        private static char GetDigit( char c ) => c switch
        {
            'B' or 'F' or 'P' or 'V' => '1',
            'C' or 'G' or 'J' or 'K' or 'Q' or 'S' or 'X' or 'Z' => '2',
            'D' or 'T' => '3',
            'L' => '4',
            'M' or 'N' => '5',
            'R' => '6',
            _ => '0'
        };
    }
}
