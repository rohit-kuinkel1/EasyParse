using EasyParser.Core;
using System.Collections.Generic;
using System.Linq;

namespace EasyParser.Suggestions
{
    internal class DidYouMean
    {
        private const double SIMILARITY_THRESHOLD = 0.6;
        private const double SOUNDEX_BOOST = 0.2;
        private const double PREFIX_BOOST = 0.15;

        private readonly ISimilarityCheck[] _metrics;

        public DidYouMean()
        {
            _metrics = new ISimilarityCheck[]
            {
                new LevenshteinSimilarity(),
                new JaroWinklerSimilarity()
            };
        }

        public IEnumerable<string> GetVerbSuggestions( string input, Verb verb )
        {
            var candidates = new List<string> { verb.VerbAttribute?.LongName ?? string.Empty };
            if( verb.VerbAttribute?.ShortName != default )
            {
                candidates.Add( verb.VerbAttribute!.ShortName.ToString() );
            }
            return GetSuggestions( input, candidates );
        }

        public IEnumerable<string> GetOptionSuggestions( string input, ICollection<Option> options )
        {
            var candidates = new List<string>();
            foreach( var option in options )
            {
                candidates.Add( option.OptionsAttribute.LongName );

                if( option.OptionsAttribute.ShortName != default( char ) )
                {
                    candidates.Add( option.OptionsAttribute.ShortName.ToString() );
                }

                if( option.OptionsAttribute.Aliases != null )
                {
                    candidates.AddRange( option.OptionsAttribute.Aliases );
                }
            }
            return GetSuggestions( input, candidates );
        }

        private IEnumerable<string> GetSuggestions( string input, IEnumerable<string> candidates )
        {
            if( string.IsNullOrWhiteSpace( input ) )
            {
                return Enumerable.Empty<string>();
            }

            input = input.ToLowerInvariant();
            var inputSoundex = SoundexProvider.GetCode( input );

            return candidates
                .Where( c => !string.IsNullOrWhiteSpace( c ) )
                .Select( candidate =>
                {
                    var lowerCandidate = candidate.ToLowerInvariant();

                    var score = _metrics.Average( m =>
                        m.Calculate( input, lowerCandidate ) );

                    if( lowerCandidate.StartsWith( input ) || input.StartsWith( lowerCandidate ) )
                    {
                        score += PREFIX_BOOST;
                    }

                    if( SoundexProvider.GetCode( lowerCandidate ) == inputSoundex )
                    {
                        score += SOUNDEX_BOOST;
                    }

                    return new { Word = candidate, Score = score };
                } )
                .Where( result => result.Score >= SIMILARITY_THRESHOLD )
                .OrderByDescending( result => result.Score )
                .ThenBy( result => result.Word.Length )
                .Take( 3 ) //tkae top 3 suggestions
                .Select( result => result.Word );
        }
    }
}