using System;

namespace EasyParse.Core
{ 
    internal class LevenshteinSimilarity : ISimilarityCheck
    {
        /// <summary>
        /// Calculates the Levenshtein similarity score between two strings.
        /// The method computes the Levenshtein distance and normalizes it by the maximum length of the two strings.
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

            var distance = CalculateDistance( source, target );
            var maxLength = Math.Max( source.Length, target.Length );

            return 1 - ( (double)distance / maxLength );
        }

        /// <summary>
        /// Calculates the Levenshtein distance between two strings.
        /// This distance represents the minimum number of single-character edits (insertions, deletions, or substitutions)
        /// required to change one string into the other.
        /// </summary>
        /// <param name="source">The source string to compare</param>
        /// <param name="target">The target string to compare</param>
        /// <returns>The Levenshtein distance between the source and target strings</returns>
        private static int CalculateDistance( string source, string target )
        {
            var n = source.Length;
            var m = target.Length;

            if( n == 0 )
            {
                return m;
            }

            if( m == 0 )
            {
                return n;
            }

            var currentRow = new int[m + 1];
            var previousRow = new int[m + 1];

            for( var j = 0; j <= m; j++ )
            {
                previousRow[j] = j;
            }

            for( var i = 0; i < n; i++ )
            {
                currentRow[0] = i + 1;

                for( var j = 0; j < m; j++ )
                {
                    var cost = ( target[j] == source[i] ) ? 0 : 1; //if characters are the same, no cost; otherwise, cost is 1
                    currentRow[j + 1] = Math.Min( Math.Min(
                        currentRow[j] + 1,        //deletion
                        previousRow[j + 1] + 1 ),  //insertion
                        previousRow[j] + cost );   //substitution
                }

                //swap the curr and prev rows for the next iteration
                var temp = previousRow;
                previousRow = currentRow;
                currentRow = temp;
            }

            return previousRow[m]; //final distance is found in the last element of the previous row
        }
    }
}
