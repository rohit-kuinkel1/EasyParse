using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyParser.Core
{
    /// <summary>
    /// Provides auto-correction functionality for command-line verbs and options.
    /// Suggests corrections for invalid inputs based on available valid options.
    /// </summary>
    internal sealed class AutoCorrector
    {
        private readonly DidYouMean _didYouMean;
        private readonly Dictionary<string, string> _verbCorrections;
        private readonly Dictionary<string, string> _optionCorrections;

        /// <summary>
        /// Initializes a new instance of the AutoCorrector class.
        /// </summary>
        public AutoCorrector()
        {
            _didYouMean = new DidYouMean();
            _verbCorrections = new Dictionary<string, string>();
            _optionCorrections = new Dictionary<string, string>();
        }

        /// <summary>
        /// Attempts to find a correction for an invalid verb from a collection of available verbs.
        /// </summary>
        /// <param name="invalidVerb">The invalid verb to correct.</param>
        /// <param name="availableVerbs">Collection of valid verbs to check against.</param>
        /// <returns>A tuple containing the matched verb and its suggestion, or null if no match is found.</returns>
        public Tuple<Verb?, string?> TryCorrectVerb( string invalidVerb, IEnumerable<Verb> availableVerbs )
        {
            foreach( var verb in availableVerbs )
            {
                var suggestions = _didYouMean.GetVerbSuggestions( invalidVerb, verb );
                if( suggestions.Any() )
                {
                    var suggestion = suggestions.First();
                    _verbCorrections[invalidVerb] = suggestion;
                    return Tuple.Create<Verb?, string?>( verb, suggestion );
                }
            }
            return Tuple.Create<Verb?, string?>( null, null );
        }

        /// <summary>
        /// Attempts to find a correction for an invalid option from a collection of available options.
        /// </summary>
        /// <param name="invalidOption">The invalid option to correct.</param>
        /// <param name="availableOptions">Collection of valid options to check against.</param>
        /// <returns>A tuple containing the matched option and its suggestion, or null if no match is found.</returns>
        public Tuple<Option?, string?> TryCorrectOption( string invalidOption, ICollection<Option> availableOptions )
        {
            foreach( var option in availableOptions )
            {
                var suggestions = _didYouMean.GetOptionSuggestions( invalidOption, new[] { option } );
                if( suggestions.Any() )
                {
                    var suggestion = suggestions.First();
                    _optionCorrections[invalidOption] = suggestion;
                    return Tuple.Create<Option?, string?>( option, suggestion );
                }
            }
            return Tuple.Create<Option?, string?>( null, null );
        }

        /// <summary>
        /// Gets whether any verb corrections have been made.
        /// </summary>
        // _verbCorrections will never be null since its instantiated in the constructor so its safe here
        public bool HasVerbCorrections => _verbCorrections.Count != 0;

        /// <summary>
        /// Gets whether any option corrections have been made.
        /// </summary>
         // _optionCorrections will never be null since its instantiated in the constructor so its safe here
        public bool HasOptionCorrections => _optionCorrections.Count != 0;

        /// <summary>
        /// Gets whether any corrections (verb or option) have been made.
        /// </summary>
        public bool HasAnyCorrections => HasVerbCorrections || HasOptionCorrections;

        /// <summary>
        /// Gets all verb corrections that have been made.
        /// </summary>
        /// <returns>A dictionary mapping invalid verbs to their corrections.</returns>
        public IReadOnlyDictionary<string, string> GetVerbCorrections() => _verbCorrections;

        /// <summary>
        /// Gets all option corrections that have been made.
        /// </summary>
        /// <returns>A dictionary mapping invalid options to their corrections.</returns>
        public IReadOnlyDictionary<string, string> GetOptionCorrections() => _optionCorrections;

        /// <summary>
        /// Gets all corrections (both verb and option) that have been made.
        /// </summary>
        /// <returns>A collection of tuples containing invalid inputs and their corrections.</returns>
        public IEnumerable<Tuple<string, string>> GetAllCorrections()
        {
            return _verbCorrections
                .Select(
                    kvp => Tuple.Create( kvp.Key, kvp.Value )
                )
                .Concat(
                    _optionCorrections.Select( kvp => Tuple.Create( kvp.Key, kvp.Value ) )
                );
        }
    }
}