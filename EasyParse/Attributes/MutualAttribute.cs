using System;
using System.Linq;
using EasyParser.Enums;

namespace EasyParser.Core
{
    /// <summary>
    /// Specifies the mutual relationship between entities (options or verbs).
    /// <para>
    /// Use case example:
    /// <code>
    ///     using EasyParse.Enums;
    ///     // Neither logFileName nor ShouldLog can/should be null
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
    [AttributeUsage( AttributeTargets.Property /*| AttributeTargets.Class | AttributeTargets.Struct*/, AllowMultiple = true )]
    public sealed class MutualAttribute : BaseAttribute
    {
        #region FieldProperties
        /// <summary>
        /// holds all the related entities for this <see cref="MutualAttribute"/>
        /// </summary>
        /// <remarks>
        /// Should be exclusively set using <see cref="RelatedEntities"/>
        /// </remarks>
        [Validated] private string[]? relatedEntities;

        /// <summary>
        /// holds all relationshipType for this <see cref="MutualAttribute"/>
        /// </summary>
        [NoValidationRequired] private MutualType relationshipType;
        #endregion

        #region AutoProperties
        /// <summary>
        /// The names of the entities that are a part of the mutual relationship.      
        /// </summary>
        /// <remarks>
        /// Holds all entities related to this <see cref="MutualAttribute"/>
        /// (both <see cref="MutualType.Exclusive"/> and <see cref="MutualType.Inclusive"/>)
        /// </remarks>
        public string[] RelatedEntities
        {
            get => relatedEntities ?? Array.Empty<string>();
            set
            {
                //if the user tries to pass null explicitly, then throw an exception
                //ArgumentNullException.ThrowIfNull( value );
                relatedEntities = value == null
                    ? Array.Empty<string>()
                    : value;
            }
        }

        /// <summary>
        /// The type of mutual relationship.
        /// <see cref="MutualType.Exclusive"/> denotes a mutually exclusive relationship, meaning only one of the property can exist/be mentioned at a given point.
        /// <see cref="MutualType.Inclusive"/> denotes a mutually inclusive relationship, meaning both of the properties have to exist/be mentioned at a given point.
        /// </summary>
        public MutualType RelationshipType
        {
            get => relationshipType;
            set => relationshipType = value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MutualAttribute"/> class.
        /// This special constructor takes <paramref name="takeRelatedEntitiesAsReference"/> which can be used
        /// to take finer control over how you want <see cref="RelatedEntities"/> to be referred to, 
        /// by reference or by a copy of the original array. When <paramref name="takeRelatedEntitiesAsReference"/>
        /// is set to <see langword="true"/> then <paramref name="relatedEntities"/> is taken by reference and any change 
        /// to the original array <paramref name="relatedEntities"/> will be reflected on <see cref="RelatedEntities"/>
        /// </summary>
        /// <param name="relationshipType">The type of mutual relationship.</param>
        /// <param name="takeRelatedEntitiesAsReference"> Specifies if the passed relatedEntities are to be taken as reference</param>
        /// <param name="relatedEntities">The names of the related options or verb.</param>
        public MutualAttribute(
            MutualType relationshipType,
            bool takeRelatedEntitiesAsReference,
            params string[] relatedEntities
        )
            : base( string.Empty, string.Empty, Array.Empty<string>() )
        {
            RelationshipType = relationshipType;
            RelatedEntities = takeRelatedEntitiesAsReference ? relatedEntities : relatedEntities.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutualAttribute"/> class.
        /// </summary>
        /// <param name="relationshipType">The type of mutual relationship.</param>
        /// <param name="relatedEntities">The names of the related options or verb.</param>
        public MutualAttribute(
            MutualType relationshipType,
            params string[] relatedEntities
        )
            : this( relationshipType, false, relatedEntities )
        {
        }
        #endregion

        #region Misc
        /// <summary>
        /// Returns a string representation of the instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var entities = string.Join( ", ", RelatedEntities );
            return $"[Mutual: {RelationshipType} with {entities}]";
        }
        #endregion
    }
}
