using System.Collections.Immutable;
using EasyParse.Tests;
using EasyParser.Utility;

namespace EasyParser.Tests
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void NotNullValidation_ValueTypes_AlwaysReturnsTrue()
        {
            int intValue = 0;
            double doubleValue = 0.0;
            bool boolValue = false;
            char charValue = '\0';
            DateTime dateValue = default;

            Assert.Multiple( () =>
            {
                Assert.That( Utility.Utility.NotNullValidation( intValue ), Is.True );
                Assert.That( Utility.Utility.NotNullValidation( doubleValue ), Is.True );
                Assert.That( Utility.Utility.NotNullValidation( boolValue ), Is.True );
                Assert.That( Utility.Utility.NotNullValidation( charValue ), Is.True );
                Assert.That( Utility.Utility.NotNullValidation( dateValue ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_NullableValueTypes_HandlesNullCorrectly()
        {
            int? nullableInt = null;
            double? nullableDouble = null;

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullableInt ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullableDouble ) );
                Assert.That( Utility.Utility.NotNullValidation( nullableInt, throwIfNull: false ), Is.False );
                Assert.That( Utility.Utility.NotNullValidation( nullableDouble, throwIfNull: false ), Is.False );
            } );
        }

        [Test]
        public void NotNullValidation_String_HandlesNullAndEmpty()
        {
            string? nullString = null;
            string emptyString = string.Empty;
            string whitespaceString = "   ";
            string validString = "test";

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullString ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptyString ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( whitespaceString ) );
                Assert.That( Utility.Utility.NotNullValidation( validString ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_String_SoftFail()
        {
            string? nullString = null;
            string emptyString = string.Empty;
            string whitespaceString = "   ";

            Assert.Multiple( () =>
            {
                Assert.That( Utility.Utility.NotNullValidation( nullString, throwIfNull: false ), Is.False );
                Assert.That( Utility.Utility.NotNullValidation( emptyString, throwIfNull: false ), Is.False );
                Assert.That( Utility.Utility.NotNullValidation( whitespaceString, throwIfNull: false ), Is.False );
            } );
        }

        [Test]
        public void NotNullValidation_Array_HandlesNullAndEmpty()
        {
            int[]? nullArray = null;
            int[] emptyArray = Array.Empty<int>();
            int[] validArray = { 1, 2, 3 };

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullArray ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptyArray ) );
                Assert.That( Utility.Utility.NotNullValidation( validArray ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_List_HandlesNullAndEmpty()
        {
            List<string>? nullList = null;
            List<string> emptyList = new();
            List<string> validList = new() { "test" };

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullList ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptyList ) );
                Assert.That( Utility.Utility.NotNullValidation( validList ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_Dictionary_HandlesNullAndEmpty()
        {
            Dictionary<string, int>? nullDict = null;
            Dictionary<string, int> emptyDict = new();
            Dictionary<string, int> validDict = new() { { "test", 1 } };

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullDict ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptyDict ) );
                Assert.That( Utility.Utility.NotNullValidation( validDict ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_CustomErrorMessage_UsesProvidedMessage()
        {
            string? nullString = null;
            const string customMessage = "Custom error message";

            var exception = Assert.Throws<NullException>( () =>
                Utility.Utility.NotNullValidation( nullString, customErrorMessage: customMessage ) );

            Assert.Multiple( () =>
            {
                Assert.That( exception, Is.Not.Null );
                Assert.That( exception?.Message.Contains( customMessage ), Is.True );
                Assert.That( exception?.Message, Is.EqualTo( "EasyParseException EasyParse  NullException, Type: System.String Custom error message" ) );
            } );
        }

        [Test]
        public void NotNullValidation_CallerInformation_IncludedInErrorMessage()
        {
            string? nullString = null;

            var exception = Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullString ) );

            Assert.Multiple( () =>
            {
                Assert.That( exception, Is.Not.Null );
                Assert.That( exception?.Message, Contains.Substring( "Parameter" ) );
                Assert.That( exception?.Message, Contains.Substring( "Called from" ) );
                Assert.That( exception?.Message, Contains.Substring( "line:" ) );
            } );
        }

        [Test]
        public void IsNullable_ValueTypes()
        {
            Assert.Multiple( () =>
            {
                Assert.That( typeof( int ).IsNullable(), Is.False );
                Assert.That( typeof( int? ).IsNullable(), Is.True );
                Assert.That( typeof( DateTime ).IsNullable(), Is.False );
                Assert.That( typeof( DateTime? ).IsNullable(), Is.True );
            } );
        }

        [Test]
        public void IsNullable_ReferenceTypes()
        {
            Assert.Multiple( () =>
            {
                Assert.That( typeof( string ).IsNullable(), Is.False );
                Assert.That( typeof( object ).IsNullable(), Is.False );
                Assert.That( typeof( List<int> ).IsNullable(), Is.False );
            } );
        }

        [Test]
        public void NotNullValidation_CustomType_HandlesNullCorrectly()
        {
            MockClass? nullCustom = null;
            MockClass validCustom = new();

            Assert.Multiple( () =>
                {
                    Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullCustom ) );
                    Assert.That( Utility.Utility.NotNullValidation( validCustom ), Is.True );
                } );
        }

        [Test]
        public void NotNullValidation_NestedCollections_HandlesEmptyCorrectly()
        {
            List<List<int>>? nullNestedList = null;
            List<List<int>> emptyOuterList = new();
            List<List<int>> emptyInnerList = new() { new List<int>() };
            List<List<int>> validNestedList = new() { new List<int> { 1 } };

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullNestedList ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptyOuterList ) );
                Assert.That( Utility.Utility.NotNullValidation( emptyInnerList ), Is.True );
                Assert.That( Utility.Utility.NotNullValidation( validNestedList ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_LinkedList_HandlesNullAndEmpty()
        {
            LinkedList<int>? nullLinkedList = null;
            LinkedList<int> emptyLinkedList = new();
            LinkedList<int> validLinkedList = new( new[] { 1, 2, 3 } );

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullLinkedList ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptyLinkedList ) );
                Assert.That( Utility.Utility.NotNullValidation( validLinkedList ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_SortedDictionary_HandlesNullAndEmpty()
        {
            SortedDictionary<string, int>? nullSortedDict = null;
            SortedDictionary<string, int> emptySortedDict = new();
            SortedDictionary<string, int> validSortedDict = new() { { "test", 1 } };

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullSortedDict ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptySortedDict ) );
                Assert.That( Utility.Utility.NotNullValidation( validSortedDict ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_SortedSet_HandlesNullAndEmpty()
        {
            SortedSet<int>? nullSortedSet = null;
            SortedSet<int> emptySortedSet = new();
            SortedSet<int> validSortedSet = new() { 1, 2, 3 };

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullSortedSet ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptySortedSet ) );
                Assert.That( Utility.Utility.NotNullValidation( validSortedSet ), Is.True );
            } );
        }

        [Test]
        public void NotNullValidation_IEnumerable_HandlesNullAndEmpty()
        {
            IEnumerable<int>? nullEnumerable = null;
            IEnumerable<int> emptyEnumerable = Enumerable.Empty<int>();
            IEnumerable<int> validEnumerable = Enumerable.Range( 1, 3 );

            Assert.Multiple( () =>
            {
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( nullEnumerable ) );
                Assert.Throws<NullException>( () => Utility.Utility.NotNullValidation( emptyEnumerable ) );
                Assert.That( Utility.Utility.NotNullValidation( validEnumerable ), Is.True );
            } );
        }
    }
}