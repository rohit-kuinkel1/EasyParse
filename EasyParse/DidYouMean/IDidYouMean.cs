namespace EasyParser.Suggestions.Metrics
{
    /// <summary>
    /// Default contract to implement for all the DYM algorithms.
    /// </summary>
    public interface IDidYouMean
    {
        /// <summary>
        /// Calculates the similarities between the <paramref name="source"/> and <paramref name="target"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public double Calculate( string source, string target );
    }
}