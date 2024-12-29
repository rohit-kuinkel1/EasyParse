using System;
using System.Collections.Generic;
using System.Linq;
using EasyParser.Core;

namespace EasyParser.Suggestions
{
    /// <summary>
    /// <see cref="DidYouMean"/> provides suggestions for mistyped command line arguments using the Levenshtein Distance.
    /// The class compares the input string to valid commands (verbs and options) and provides the most similar matches.
    /// </summary>
    public class DidYouMean
    {
        private const double SIMILARITY_THRESHOLD = 0.8;

        /// <summary>
        /// Gets suggestions for a potentially mistyped verb.
        /// This method compares the input verb to a list of valid verbs and returns the most similar matches based on Levenshtein Distance.
        /// The comparison includes both the long name and short name of the verb.
        /// </summary>
        /// <param name="input">The potentially mistyped verb</param>
        /// <param name="verb">The verb store containing valid commands</param>
        /// <returns>A list of suggested corrections, or empty if no good matches</returns>
        public IEnumerable<string> GetVerbSuggestions( string input, Verb verb )
        {
            var candidates = new List<string> { verb.VerbAttribute?.LongName ?? string.Empty };

            //add short name if it exists
            if( verb.VerbAttribute?.ShortName != default )
            {
                candidates.Add( verb.VerbAttribute!.ShortName.ToString() );
            }

            return GetSuggestions( input, candidates );
        }

        /// <summary>
        /// Gets suggestions for a potentially mistyped option.
        /// This method compares the input option to a collection of valid options and returns the most similar matches.
        /// It considers long names, short names, and aliases of options.
        /// </summary>
        /// <param name="input">The potentially mistyped option</param>
        /// <param name="options">The collection of valid options</param>
        /// <returns>A list of suggested corrections, or empty if no good matches</returns>
        public IEnumerable<string> GetOptionSuggestions( string input, ICollection<Option> options )
        {
            var candidates = new List<string>();

            foreach( var option in options )
            {
                //add all the long names for options
                candidates.Add( option.OptionsAttribute.LongName );

                //add short names if present
                if( option.OptionsAttribute.ShortName != default( char ) )
                {
                    candidates.Add( option.OptionsAttribute.ShortName.ToString() );
                }

                //add the aliases as well
                if( option.OptionsAttribute.Aliases != null )
                {
                    candidates.AddRange( option.OptionsAttribute.Aliases );
                }
            }

            return GetSuggestions( input, candidates );
        }

        /// <summary>
        /// Gets suggestions for the mistyped input by comparing it against a list of candidate strings.
        /// It calculates the Levenshtein Distance between the input and each candidate and returns the most similar matches.
        /// Only candidates with a similarity score above the predefined threshold are returned.
        /// The results are ordered by similarity, with the most similar suggestions appearing first.
        /// </summary>
        /// <param name="input">The potentially mistyped string</param>
        /// <param name="candidates">A collection of valid candidate strings to compare with</param>
        /// <returns>A list of suggested corrections, or empty if no good matches</returns>
        private IEnumerable<string> GetSuggestions( string input, IEnumerable<string> candidates )
        {
            //string.IsNullOrEmpty
            if( string.IsNullOrWhiteSpace( input ) )
            {
                return Enumerable.Empty<string>();
            }

            return candidates
                .Where( c => !string.IsNullOrWhiteSpace( c ) )
                .Select( candidate => new
                {
                    Word = candidate,
                    Similarity = CalculateSimilarity( input.ToLowerInvariant(), candidate.ToLowerInvariant() )
                } )
                .Where( result => result.Similarity >= SIMILARITY_THRESHOLD )
                .OrderByDescending( result => result.Similarity )
                .Take( 3 ) //take the top 3 suggestions
                .Select( result => result.Word );
        }

        /// <summary>
        /// Calculates a normalized similarity score between two strings using Levenshtein Distance.
        /// The similarity is calculated as 1 - (Levenshtein Distance / max(string lengths)).
        /// The result is a value between 0 and 1, where 1 means the strings are identical and 0 means they are completely different.
        /// </summary>
        /// <param name="source">The first string to compare</param>
        /// <param name="target">The second string to compare</param>
        /// <returns>A similarity score between 0 and 1</returns>
        private double CalculateSimilarity( string source, string target )
        {
            var distance = LevenshteinDistance( source, target );
            var maxLength = Math.Max( source.Length, target.Length );

            return 1 - ( (double)distance / maxLength );
        }

        /// <summary>
        /// Calculates the Levenshtein Distance, a metric for measuring the difference between two strings.
        /// The LD represents the minimum number of single-character edits (insertions, deletions, or substitutions) required 
        /// to transform one string into another. The algorithm uses DP to build a matrix where each cell 
        /// represents the cost of transforming substrings of the two strings.
        /// <para>
        /// 1. If either of the strings is empty, the distance is simply the length of the other string.
        /// 2. The matrix is initialized where the first row and first column represent the base case, i.e., the cost of 
        ///    converting an empty string to a substring of the other string.
        /// 3. The matrix is then filled by calculating the cost of insertion, deletion, or substitution at each step. 
        ///    The value in each cell is the minimum cost from the three possible operations.
        /// 4. Finally, the value in the bottom-right corner of the matrix is returned as the Levenshtein distance, 
        ///    which represents the minimum edit distance between the two input strings.
        /// </para>
        /// </summary>
        private int LevenshteinDistance( string source, string target )
        {
            var n = source.Length;
            var m = target.Length;
            var d = new int[n + 1, m + 1];

            //initialize the matrix
            if( n == 0 )
            {
                return m;
            }

            if( m == 0 )
            {
                return n;
            }

            //each comparison will gets its own 2D array
            for( var i = 0; i <= n; i++ )
            {
                d[i, 0] = i;
            }

            for( var j = 0; j <= m; j++ )
            {
                d[0, j] = j;
            }

            //build the matrix
            for( var i = 1; i <= n; i++ )
            {
                for( var j = 1; j <= m; j++ )
                {
                    var cost = ( target[j - 1] == source[i - 1] ) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min( d[i - 1, j] + 1, d[i, j - 1] + 1 ),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            //return the bottom-right cell
            return d[n, m];
        }
    }
}