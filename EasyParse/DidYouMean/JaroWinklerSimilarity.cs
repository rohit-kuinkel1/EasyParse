using System;

namespace EasyParse.Core
{
    internal class JaroWinklerSimilarity : ISimilarityCheck
    {
        private const double PREFIX_SCALE = 0.1;

        /// <summary>
        /// Calculates the Jaro-Winkler similarity between two strings.
        /// The method first calculates the Jaro similarity and then adjusts it based on the common prefix length.
        /// </summary>
        /// <param name="source">The source string to compare</param>
        /// <param name="target">The target string to compare</param>
        /// <returns>A similarity score between 0 and 1, where 1 indicates identical strings</returns>
        public double Calculate( string source, string target )
        {
            var check1 = EasyParse.Utility.Utility.NotNullValidation( source, throwIfNull: false );
            var check2 = EasyParse.Utility.Utility.NotNullValidation( target, throwIfNull: false );
            if( !check1 || !check2 )
            {
                return 0.0;
            }

            var jaroSim = CalculateJaroSimilarity( source, target );
            var prefixLength = GetCommonPrefixLength( source, target );
            return jaroSim + ( prefixLength * PREFIX_SCALE * ( 1 - jaroSim ) );
        }

        /// <summary>
        /// Calculates the Jaro similarity between two strings.
        /// This is the core of the Jaro-Winkler distance and is based on matching characters and transpositions.
        /// </summary>
        /// <param name="source">The source string to compare</param>
        /// <param name="target">The target string to compare</param>
        /// <returns>A similarity score between 0 and 1 based on the number of matches and transpositions</returns>
        private static double CalculateJaroSimilarity( string source, string target )
        {
            //if both have the len of 0 then return 1
            if( source.Length == 0 && target.Length == 0 )
            {
                return 1.0;
            }

            //if either one of them is empty then that means the other one is not empty since we reached here, so there is no similarity, return 0
            if( source.Length == 0 || target.Length == 0 )
            {
                return 0.0;
            }

            var matchDistance = Math.Max( source.Length, target.Length ) / 2 - 1;
            var sourceMatches = new bool[source.Length];
            var targetMatches = new bool[target.Length];

            var matches = FindMatches( source, target, matchDistance, sourceMatches, targetMatches );
            if( matches == 0 )
            {
                return 0.0;
            }

            var transpositions = CountTranspositions( source, target, sourceMatches, targetMatches );

            var calculatedSimilarity = CalculateSimilarity( matches, transpositions, source.Length, target.Length );
            return calculatedSimilarity;
        }

        /// <summary>
        /// Finds the matching characters between the source and target strings, within a given match distance.
        /// The match distance defines the window in which characters from the source and target can match.
        ///
        /// This method iterates over each character in the source string and looks for a matching character in
        /// the target string, within the range defined by the match distance. A match is recorded when both
        /// characters are the same and neither has been previously matched.
        /// </summary>
        /// <remarks>
        /// See <see cref="CalculateJaroSimilarity(string, string)"/>
        /// </remarks>
        /// <param name="source">The source string to compare.</param>
        /// <param name="target">The target string to compare.</param>
        /// <param name="matchDistance">The maximum distance within which matches can occur.</param>
        /// <param name="sourceMatches">An array tracking which characters in the source string have been matched.</param>
        /// <param name="targetMatches">An array tracking which characters in the target string have been matched.</param>
        /// <returns>The number of matching characters between the source and target strings.</returns>
        private static int FindMatches( string source, string target, int matchDistance, bool[] sourceMatches, bool[] targetMatches )
        {
            var matches = 0;
            for( var i = 0; i < source.Length; i++ )
            {
                var start = Math.Max( 0, i - matchDistance );
                var end = Math.Min( i + matchDistance + 1, target.Length );
                for( var j = start; j < end; j++ )
                {
                    if( !targetMatches[j] && source[i] == target[j] )
                    {
                        sourceMatches[i] = true;
                        targetMatches[j] = true;
                        matches++;
                        break;
                    }
                }
            }
            return matches;
        }

        /// <summary>
        /// Counts the number of transpositions between the matched characters in the source and target strings.
        /// A transposition occurs when two characters are matched but in different positions in the source and target.
        ///
        /// This method compares the positions of matched characters in the source and target strings to determine
        /// how many transpositions occurred. A transposition is only counted if the characters are not in the same order.
        ///
        /// </summary>
        /// <remarks>
        /// See <see cref="CalculateJaroSimilarity(string, string)"/>
        /// </remarks>
        /// <param name="source">The source string to compare.</param>
        /// <param name="target">The target string to compare.</param>
        /// <param name="sourceMatches">An array tracking which characters in the source string have been matched.</param>
        /// <param name="targetMatches">An array tracking which characters in the target string have been matched.</param>
        /// <returns>The number of transpositions in the matched characters between the source and target strings.</returns>
        private static int CountTranspositions( string source, string target, bool[] sourceMatches, bool[] targetMatches )
        {
            var transpositions = 0;
            var k = 0;
            for( var i = 0; i < source.Length; i++ )
            {
                if( !sourceMatches[i] ) continue;
                while( !targetMatches[k] ) k++;
                if( source[i] != target[k] ) transpositions++;
                k++;
            }
            return transpositions;
        }

        /// <summary>
        /// Calculates the final Jaro similarity score based on the number of matches and transpositions between
        /// the source and target strings, as well as their lengths.
        ///
        /// The similarity score is calculated using the formula:
        /// (matches / sourceLength + matches / targetLength + (matches - transpositions / 2) / matches) / 3
        /// This gives an average of three components: match density, length-based match, and transposition penalty.
        /// </summary>
        /// <remarks>
        /// See <see cref="CalculateJaroSimilarity(string, string)"/>
        /// </remarks>
        /// <param name="matches">The number of matching characters between the source and target strings.</param>
        /// <param name="transpositions">The number of transpositions (misordered matches) between the source and target.</param>
        /// <param name="sourceLength">The length of the source string.</param>
        /// <param name="targetLength">The length of the target string.</param>
        /// <returns>The Jaro similarity score, a value between 0 and 1 indicating the similarity between the strings.</returns>
        private static double CalculateSimilarity( int matches, int transpositions, int sourceLength, int targetLength )
        {
            return
                (
                    (double)matches / sourceLength +
                    (double)matches / targetLength +
                    (double)( matches - transpositions / 2 ) / matches
                ) / 3.0;
        }


        /// <summary>
        /// Gets the length of the common prefix between two strings.
        /// The common prefix is given a higher weight in the Jaro-Winkler algorithm.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="target">The target string</param>
        /// <returns>The length of the common prefix</returns>
        private static int GetCommonPrefixLength( string source, string target )
        {
            var maxLength = Math.Min( source.Length, target.Length );
            var i = 0;

            //find the longest common prefix
            while( i < maxLength && source[i] == target[i] )
            {
                i++;
            }
            return i;
        }
    }
}
