using System;

namespace EasyParser.Core
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
            //var check1 = EasyParser.Utility.Utility.NotNullValidation( source, throwIfNull: false );
            //var check2 = EasyParser.Utility.Utility.NotNullValidation( target, throwIfNull: false );
            //if( !check1 || !check2 )
            //{
            //    return 0.0;
            //}

            if( string.IsNullOrEmpty( source ) || string.IsNullOrEmpty( target ) )
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
        private double CalculateJaroSimilarity( string source, string target )
        {
            if( source.Length == 0 && target.Length == 0 )
            {
                return 1.0;
            }

            if( source.Length == 0 || target.Length == 0 )
            {
                return 0.0;
            }

            var matchDistance = Math.Max( source.Length, target.Length ) / 2 - 1;
            var sourceMatches = new bool[source.Length];
            var targetMatches = new bool[target.Length];
            var matches = 0;
            var transpositions = 0;

            //find matches between the source and target strings
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

            if( matches == 0 )
            {
                return 0.0;
            }

            //now lets count the transpositions
            var k = 0;
            for( var i = 0; i < source.Length; i++ )
            {
                if( !sourceMatches[i] ) continue;
                while( !targetMatches[k] ) k++;
                if( source[i] != target[k] ) transpositions++;
                k++;
            }

            return
                (
                    (double)matches / source.Length +
                    (double)matches / target.Length +
                    (double)( matches - transpositions / 2 )
                    / matches
                ) / 3.0;
        }

        /// <summary>
        /// Gets the length of the common prefix between two strings.
        /// The common prefix is given a higher weight in the Jaro-Winkler algorithm.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="target">The target string</param>
        /// <returns>The length of the common prefix</returns>
        private int GetCommonPrefixLength( string source, string target )
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
