using EasyParse.Core;
using EasyParse.Enums;

//testing attributes by themselves is a bit weird but im gonna do it anyways
namespace EasyParse.Tests.Core
{
    [TestFixture]
    public class MutualAttributeTests
    {
        [Test]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
        {
            MutualType relationshipType = MutualType.Inclusive;
            var relatedEntities = new[] { "option1", "option2", "option3", "option4", "option5", "option6" };

            var attribute = new MutualAttribute( relationshipType, relatedEntities );

            Assert.Multiple( () =>
            {
                Assert.That( attribute.RelationshipType, Is.EqualTo( relationshipType ) );
                Assert.That( attribute.RelatedEntities, Is.EqualTo( relatedEntities ) );
            } );
        }

        [Test]
        public void Constructor_WithEmptyRelatedEntities_CreatesInstanceWithEmptyArray()
        {
            var attribute = new MutualAttribute( MutualType.Exclusive );

            Assert.Multiple( () =>
            {
                Assert.That( attribute.RelatedEntities, Is.Not.Null );
                Assert.That( attribute.RelatedEntities, Has.Length.EqualTo( 0 ) );
            } );
        }

        [Test]
        public void Constructor_WithNullRelatedEntities_ThrowsArgumentNullException()
        {
            //if you use the constructor for MutualAttribute, then you need to provide both; relation type as well as the related entities
            //and since relationType is marked non nullable its not possible to pass null to an enum of non nullable type so we skip that check
            Assert.Throws<ArgumentNullException>( () => new MutualAttribute( MutualType.Inclusive, null! ) );
        }

        [Test]
        public void ToString_WithSingleRelatedEntity_ReturnsCorrectFormat()
        {
            var attribute = new MutualAttribute( MutualType.Exclusive, "option1" );

            var result = attribute.ToString();

            Assert.That( result, Is.EqualTo( "[Mutual: Exclusive with option1]" ) );
        }

        [Test]
        public void ToString_WithMultipleRelatedEntities_ReturnsCorrectFormat()
        {
            var attribute = new MutualAttribute( MutualType.Inclusive, "option1", "option2", "option3", "option4", "option5", "option6" );

            var result = attribute.ToString();

            Assert.That( result, Is.EqualTo( "[Mutual: Inclusive with option1, option2, option3, option4, option5, option6]" ) );
        }

        [Test]
        public void AttributeUsage_AllowsMultipleInstances()
        {
            var attributeType = typeof( MutualAttribute );

            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute( attributeType, typeof( AttributeUsageAttribute ) )!;

            Assert.That( usage.AllowMultiple, Is.True, "It should be possible to decorate a class property with Multiple MutualAttribute" );
        }

        [Test]
        public void AttributeUsage_HasCorrectTargets()
        {
            var attributeType = typeof( MutualAttribute );

            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
                attributeType, typeof( AttributeUsageAttribute ) )!;

            var expectedTargets = AttributeTargets.Property;
            Assert.That( usage.ValidOn, Is.EqualTo( expectedTargets ), "MutualAttribute should target class properties only; for now!" );
        }

        [TestCase( MutualType.Exclusive )]
        [TestCase( MutualType.Inclusive )]
        public void Constructor_WithDifferentMutualTypes_SetsTypeCorrectly( MutualType type )
        {
            var attribute = new MutualAttribute( type, "option1" );
            Assert.That( attribute.RelationshipType, Is.EqualTo( type ) );
        }

        [Test]
        public void RelatedEntities_IsImmutable()
        {
            var originalEntities = new[] { "option1", "option2" };
            var attribute = new MutualAttribute( MutualType.Inclusive, originalEntities );

            originalEntities[0] = "modified";

            Assert.That( attribute.RelatedEntities[0], Is.EqualTo( "option1" ), "RelatedEntities array should be a copy of the array, not referencing the original array" );
        }

        [Test]
        public void RelatedEntities_IsMutableWhenRightConstructorIsUsed()
        {
            var originalEntities = new[] { "option1", "option2" };
            var attribute = new MutualAttribute( relationshipType: MutualType.Inclusive, takeRelatedEntitiesAsReference: true, originalEntities );

            originalEntities[0] = "modified";

            Assert.That( attribute.RelatedEntities[0], Is.EqualTo( "modified" ), "RelatedEntities array should be referencing the original array, not a copy of the array" );
        }
    }
}