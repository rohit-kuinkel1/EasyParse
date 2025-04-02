using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyParse.Core
{
    /// <summary>
    /// <see cref="ParsingResultStore"/> represents a store or collection of all the <see cref="ParsingResult{T}"/>.
    /// It contains properties like <see cref="_mentionedInstances"/> which holds all the <see cref="ParsingResult{T}"/>
    /// </summary>
    public class ParsingResultStore
    {
        /// <summary>
        /// Store for all the <see cref="ParsingResult{T}"/> instances
        /// </summary>
        private readonly ICollection<object> _mentionedInstances;

        /// <summary>
        /// Store for successfully parsed <see cref="ParsingResult{T}"/> instances
        /// </summary>
        private readonly ICollection<object> _parsedInstances;

        /// <summary>
        /// Default constructor for <see cref="ParsingResultStore"/>.
        /// Instantiates <see cref="_mentionedInstances"/> with a new <see cref="List{T}"/> where 
        /// <typeparamref name="T"/> is of type <see langword="object"/>.
        /// </summary>
        internal ParsingResultStore()
        {
            _mentionedInstances = new HashSet<object>();
            _parsedInstances = new HashSet<object>();
        }

        /// <summary>
        /// <see cref="AddResult{T}(ParsingResult{T})"/> adds a <see cref="ParsingResult{T}"/> passed as param
        /// to the <see cref="_mentionedInstances"/> using the <see cref="List{T}.Add(T)"/> method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        internal void AddResult<T>( ParsingResult<T> result ) where T : class, new()
        {
            _mentionedInstances.Add( result );

            //if the Success was set to true, then that means the input arg contained elements for the class so it was parsed, if that makes sense
            if( result.Success )
            {
                _parsedInstances.Add( result );
            }
        }

        /// <summary>
        /// <see cref="GetResults{T}"/> gets a specific <see cref="ParsingResult{T}"/>
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<ParsingResult<T>> GetResults<T>() where T : class, new()
        {
            return _mentionedInstances.OfType<ParsingResult<T>>();
        }

        /// <summary>
        /// <see cref="MentionedInstances"/> gets all the <see cref="ParsingResult{T}"/> instances
        /// stored in <see cref="_mentionedInstances"/>
        /// </summary>
        /// <remarks>
        /// It returns a <see cref="ICollection{T}"/> of all the <see cref="ParsingResult{T}"/> instances.
        /// Use <see cref="ParsedInstances"/> to get all the succesfully parsed instances.
        /// </remarks>
        public IEnumerable<object> MentionedInstances => _mentionedInstances;

        /// <summary>
        /// <see cref="ParsedInstances"/> gets all the parsed <see cref="ParsingResult{T}"/> instances
        /// stored in <see cref="_parsedInstances"/>.
        /// </summary>
        /// <remarks>
        /// It only returns a <see cref="ICollection{T}"/> of <see cref="ParsingResult{T}"/> where
        /// the <see cref="ParsingResult{T}.Success"/> property was set to <see langword="true"/>
        /// by a <see cref="EasyParse.Parsing.BaseParsing"/> instance.
        /// Use <see cref="MentionedInstances"/> to get all the mentioned instances
        /// </remarks>
        public IEnumerable<object> ParsedInstances => _parsedInstances;


        /// <summary>
        /// <see cref="Successes"/> is an auto getter for all the <see cref="ParsingResult{T}.Success"/> property 
        /// from each <see cref="ParsingResult{T}"/> stored in the store <see cref="_mentionedInstances"/>
        /// </summary>
        public IEnumerable<object?> Successes =>
            _mentionedInstances.OfType<dynamic>()
                            .Select( result => result.Success );

        /// <summary>
        /// <see cref="Errors"/> is an auto getter for all the <see cref="ParsingResult{T}.ErrorMessage"/> property 
        /// from each <see cref="ParsingResult{T}"/> stored in the store <see cref="_mentionedInstances"/>
        /// </summary>
        public IEnumerable<string?> Errors =>
            _mentionedInstances.OfType<dynamic>()
                 .Where( result => !result.Success )
                 .Select( result => (string?)result.ErrorMessage?.ToString() );

        /// <summary>
        /// <see cref="Successes"/> is an auto getter property indicating whether all parsing results were successful
        /// or not. It uses the extension method <see cref="Enumerable.All{TSource}(IEnumerable{TSource}, System.Func{TSource, bool})"/> 
        /// for <see cref="Successes"/> to see if all the values in <see cref="Successes"/>
        /// were <see langword="true"/> or not.
        /// </summary>
        public bool Success => Successes?.All( s => s is not null && (bool)s ) ?? false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach( var parsedInstance in _mentionedInstances )
            {
                stringBuilder.AppendLine( parsedInstance.ToString() );
            }
            return stringBuilder.ToString();
        }
    }
}