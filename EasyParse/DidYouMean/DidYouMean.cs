using System.Collections.Generic;
using System.Linq;

namespace EasyParser.Core
{
    internal sealed class DidYouMean
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
            if( string.IsNullOrWhiteSpace( input ) )
            {
                return Enumerable.Empty<string>();
            }

            //verbs dont have aliases by design so we dont have to worry about them and create a dict, here a HashSet suffices
            var allNames = new HashSet<string> { verb.VerbAttribute?.LongName ?? string.Empty };
            if( verb.VerbAttribute?.ShortName != default )
            {
                allNames.Add( verb.VerbAttribute!.ShortName.ToString() );
            }

            input = input.ToLowerInvariant();
            var inputSoundex = SoundexProvider.GetCode( input );

            var matches = allNames
                .Where( c => !string.IsNullOrWhiteSpace( c ) )
                .Select( candidate =>
                {
                    var currentCandidate = candidate.ToLowerInvariant();
                    var score = _metrics.Average( m =>
                        m.Calculate( input, currentCandidate ) );

                    if( currentCandidate.StartsWith( input ) || input.StartsWith( currentCandidate ) )
                    {
                        score += PREFIX_BOOST;
                    }

                    if( SoundexProvider.GetCode( currentCandidate ) == inputSoundex )
                    {
                        score += SOUNDEX_BOOST;
                    }

                    return new { Candidate = candidate, Score = score };
                } )
                .Where( result => result.Score >= SIMILARITY_THRESHOLD )
                .OrderByDescending( result => result.Score )
                .Take( 3 );

            return matches.Any() ? allNames : Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetOptionSuggestions( string originalInput, ICollection<Option> options )
        {
            if( string.IsNullOrWhiteSpace( originalInput ) )
            {
                return Enumerable.Empty<string>();
            }

            var optionGroups = new Dictionary<string, HashSet<string>>();

            foreach( var option in options )
            {
                var allNames = new HashSet<string> { option.OptionsAttribute.LongName };

                if( option.OptionsAttribute.ShortName != default( char ) )
                {
                    allNames.Add( option.OptionsAttribute.ShortName.ToString() );
                }

                if( option.OptionsAttribute.Aliases != null )
                {
                    foreach( var alias in option.OptionsAttribute.Aliases )
                    {
                        allNames.Add( alias );
                    }
                }

                optionGroups[option.OptionsAttribute.LongName] = allNames;
            }

            originalInput = originalInput.ToLowerInvariant();
            var inputSoundex = SoundexProvider.GetCode( originalInput );

            var allCandidates = optionGroups.SelectMany( g => g.Value )
                .Where( c => !string.IsNullOrWhiteSpace( c ) );

            var matches = allCandidates
                .Select( candidate =>
                {
                    var currentCandidate = candidate.ToLowerInvariant();
                    var score = _metrics.Average( m =>
                        m.Calculate( originalInput, currentCandidate )
                    );

                    if( currentCandidate.StartsWith( originalInput ) || originalInput.StartsWith( currentCandidate ) )
                    {
                        score += PREFIX_BOOST;
                    }

                    if( SoundexProvider.GetCode( currentCandidate ) == inputSoundex )
                    {
                        score += SOUNDEX_BOOST;
                    }

                    return new { Candidate = candidate, Score = score };
                } )
                .Where( result => result.Score >= SIMILARITY_THRESHOLD )
                .OrderByDescending( result => result.Score )
                .Take( 3 );

            var suggestedGroups = matches
                .Select( m => optionGroups.FirstOrDefault( g => g.Value.Contains( m.Candidate ) ) )
                .Where( g => !string.IsNullOrEmpty( g.Key ) )
                .Select( g => g.Value )
                .Distinct();

            return suggestedGroups.SelectMany( g => g ); //return all without restricting to top 3 suggestions here since we know for a fact all are correct (pure)
        }
    }
}