using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParser.Core;

namespace EasyParser.Misc
{
    /// <summary>
    /// <see cref="VerbParsingUtility"/> helps with multi parsing of the <see cref="VerbAttribute"/>.
    /// When the user provides multiple verbs within a single go, then <see cref="VerbParsingUtility"/>
    /// helps to parse the input in a more efficient manner by aiming to reduce reflections or having to
    /// call <see cref="EasyParser.Parsing.BaseParsing.ParseOne{T}(string[])"/> for each template T 
    /// passed to <see cref="EasyParser.EasyParseExtensions.Parse{T1}(EasyParse, string[])"/> where it can
    /// take multiple templates like T1.
    /// </summary>
    internal static class VerbParsingUtility
    {
        private const string VERB_SEPARATOR = "&";

        /// <summary>
        /// Splits arguments into groups based on verb separator
        /// </summary>
        public static string[][] SplitVerbGroups( string[] args )
        {
            var groups = new List<string[]>();
            var currentGroup = new List<string>();

            foreach( var arg in args )
            {
                if( string.Equals( arg, VERB_SEPARATOR ) )
                {
                    if( currentGroup.Count > 0 )
                    {
                        groups.Add( currentGroup.ToArray() );
                        currentGroup.Clear();
                    }
                }
                else
                {
                    currentGroup.Add( arg );
                }
            }

            //without this statement, the last group of arguments would be missed if it doesnt end with VERB_SEPARATOR so this helps us collect all of the parsed string.
            if( currentGroup.Count > 0 )
            {
                groups.Add( currentGroup.ToArray() );
            }

            return groups.ToArray();
        }

        /// <summary>
        /// Gets the verb name from the first argument in the command
        /// </summary>
        public static string GetVerbName( string[] args )
        {
            return args.Length > 0 ? args[0].ToLowerInvariant() : string.Empty;
        }

        /// <summary>
        /// Checks if the type matches the provided verb name based on VerbAttribute
        /// </summary>
        public static bool IsMatchingVerbType<T>( string verbName ) where T : class, new()
        {
            var verbAttribute = typeof( T ).GetCustomAttribute<VerbAttribute>();
            if( verbAttribute == null )
            {
                return false;
            }

            return verbAttribute.LongName.Equals( verbName, StringComparison.OrdinalIgnoreCase ) ||
                   verbAttribute.ShortName.ToString().Equals( verbName, StringComparison.OrdinalIgnoreCase ) ||
                   verbAttribute.Aliases.Any( alias => alias.Equals( verbName, StringComparison.OrdinalIgnoreCase ) );
        }
    }
}
