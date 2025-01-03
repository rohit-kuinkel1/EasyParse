using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyParser.Core
{
    /// <summary>
    /// <see cref="ParsingResultStore"/> represents a store or collection of all the <see cref="ParsingResult{T}"/>.
    /// It contains properties like <see cref="_parsedInstances"/> which holds all the <see cref="ParsingResult{T}"/>
    /// </summary>
    public class ParsingResultStore
    {
        /// <summary>
        /// Store for all the <see cref="ParsingResult{T}"/> instances
        /// </summary>
        private readonly ICollection<object> _parsedInstances;

        /// <summary>
        /// Default constructor for <see cref="ParsingResultStore"/>.
        /// Instantiates <see cref="_parsedInstances"/> with a new <see cref="List{T}"/> where 
        /// <typeparamref name="T"/> is of type <see langword="object"/>.
        /// </summary>
        internal ParsingResultStore()
        {
            _parsedInstances = new List<object>();
        }

        /// <summary>
        /// <see cref="AddResult{T}(ParsingResult{T})"/> adds a <see cref="ParsingResult{T}"/> passed as param
        /// to the <see cref="_parsedInstances"/> using the <see cref="List{T}.Add(T)"/> method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        internal void AddResult<T>( ParsingResult<T> result ) where T : class, new()
        {
            _parsedInstances.Add( result );
        }

        /// <summary>
        /// <see cref="GetResults{T}"/> gets a specific <see cref="ParsingResult{T}"/>
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<ParsingResult<T>> GetResults<T>() where T : class, new()
        {
            return _parsedInstances.OfType<ParsingResult<T>>();
        }

        /// <summary>
        /// <see cref="Successes"/> is an auto getter for all the <see cref="ParsingResult{T}.Success"/> property 
        /// from each <see cref="ParsingResult{T}"/> stored in the store <see cref="_parsedInstances"/>
        /// </summary>
        public IEnumerable<object?> Successes =>
            _parsedInstances.OfType<dynamic>()
                            .Select( result => result.Success );

        /// <summary>
        /// <see cref="Errors"/> is an auto getter for all the <see cref="ParsingResult{T}.ErrorMessage"/> property 
        /// from each <see cref="ParsingResult{T}"/> stored in the store <see cref="_parsedInstances"/>
        /// </summary>
        public IEnumerable<string?> Errors =>
            _parsedInstances.OfType<dynamic>()
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
            foreach( var parsedInstance in _parsedInstances )
            {
                stringBuilder.AppendLine( parsedInstance.ToString() );
            }
            return stringBuilder.ToString();
        }
    }
}