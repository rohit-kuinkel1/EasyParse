using System;
using EasyParser.Enums;

namespace EasyParser.Core
{
    /// <summary>
    /// Specifies the mutual relationship between entities (options or verbs).
    /// <para>
    /// Use case example:
    /// <code>
    ///     using EasyParse.Enums;
    ///     // Neither logFileName nor ShouldLog can be null
    ///     [Mutual(MutualType.Inclusive, nameof(ShouldLog))]
    ///     public string logFileName;
    /// </code>
    /// </para>
    /// <para>
    ///     <see cref="MutualType.Exclusive"/> indicates a mutually exclusive relationship, meaning one of the related entities must not be defined at a given time. 
    /// </para>
    /// <para>
    ///     <see cref="MutualType.Inclusive"/> indicates a mutually inclusive relationship, meaning both related entities must be defined at a given time.
    /// </para>
    /// </summary>

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true )]
    public sealed class MutualAttribute : BaseAttribute
    {
        /// <summary>
        /// The name of the entity that is part of the mutual relationship.
        /// </summary>
        public string[] RelatedEntities { get; }

        /// <summary>
        /// The type of mutual relationship.
        /// <see cref="MutualType.Exclusive"/> denotes a mutually exclusive relationship, meaning only one can exist at a given point.
        /// <see cref="MutualType.Inclusive"/> denotes a mutually inclusive relationship, meaning both have to exist at a given point.
        /// </summary>
        public MutualType RelationshipType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutualAttribute"/> class.
        /// </summary>
        /// <param name="relationshipType">The type of mutual relationship.</param>
        /// <param name="relatedEntities">The names of the related options or verb.</param>
        public MutualAttribute( MutualType relationshipType, params string[] relatedEntities )
        : base( string.Empty, Array.Empty<string>() )
        {
            RelatedEntities = relatedEntities;
            RelationshipType = relationshipType;
        }
        
        /// <summary>
        /// Returns a string representation of the instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var entities = string.Join( ", ", RelatedEntities );
            return $"[Mutual: {RelationshipType} with {entities}]";
        }
    }
}
