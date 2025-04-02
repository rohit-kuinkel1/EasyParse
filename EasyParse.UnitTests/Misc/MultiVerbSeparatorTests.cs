using EasyParse.Core;
using EasyParse.Misc;

namespace EasyParse.MiscTests
{
    [TestFixture]
    public class VerbParsingUtilityTests
    {
        [Test]
        public void SplitVerbGroups_WithMultipleGroups_ShouldSplitCorrectly()
        {
            var args = new[] { "add", "--read", "file.txt", "-t", "test123", "&", "process", "--input", "video.mp4", "-r", "1080p", "&", "backup", "--host", "localhost", "--ip", "192.168.1.1" };
            var result = VerbParsingUtility.SplitVerbGroups( args );

            Assert.Multiple( () =>
            {
                Assert.That( result.Length, Is.EqualTo( 3 ) );
                Assert.That( result[0], Is.EqualTo( new[] { "add", "--read", "file.txt", "-t", "test123" } ) );
                Assert.That( result[1], Is.EqualTo( new[] { "process", "--input", "video.mp4", "-r", "1080p" } ) );
                Assert.That( result[2], Is.EqualTo( new[] { "backup", "--host", "localhost", "--ip", "192.168.1.1" } ) );
            } );
        }

        [Test]
        public void SplitVerbGroups_WithSingleGroup_ShouldReturnOneGroup()
        {
            var args = new[] { "add", "--read", "file.txt", "-t", "test123" };
            var result = VerbParsingUtility.SplitVerbGroups( args );

            Assert.Multiple( () =>
            {
                Assert.That( result.Length, Is.EqualTo( 1 ) );
                Assert.That( result[0], Is.EqualTo( args ) );
            } );
        }

        [Test]
        public void SplitVerbGroups_WithEmptyInput_ShouldReturnEmptyArray()
        {
            var result = VerbParsingUtility.SplitVerbGroups( Array.Empty<string>() );

            Assert.Multiple( () =>
            {
                Assert.DoesNotThrow( () => VerbParsingUtility.SplitVerbGroups( Array.Empty<string>() ) );
                Assert.That( result, Is.Empty );
            } );
        }

        [Test]
        public void SplitVerbGroups_WithConsecutiveSeparators_ShouldHandleCorrectly()
        {
            var args = new[] { "add", "--read", "file.txt", "-t", "test123", "&", "&", "process", "--input", "video.mp4", "-r", "1080p" };
            var result = VerbParsingUtility.SplitVerbGroups( args );

            Assert.Multiple( () =>
            {
                Assert.That( result.Length, Is.EqualTo( 2 ) );
                Assert.That( result[0], Is.EqualTo( new[] { "add", "--read", "file.txt", "-t", "test123" } ) );
                Assert.That( result[1], Is.EqualTo( new[] { "process", "--input", "video.mp4", "-r", "1080p" } ) );
            } );
        }

        [Test]
        public void GetVerbName_WithValidArgs_ShouldReturnFirstArgument()
        {
            var args = new[] { "add", "--read", "file.txt" };
            var result = VerbParsingUtility.GetVerbName( args );
            Assert.That( result, Is.EqualTo( "add" ) );
        }

        [Test]
        public void GetVerbName_WithEmptyArgs_ShouldReturnEmptyString()
        {
            var result = VerbParsingUtility.GetVerbName( Array.Empty<string>() );
            Assert.That( result, Is.EqualTo( string.Empty ) );
        }

        [Test]
        public void GetVerbName_WithMixedCase_ShouldReturnLowerCase()
        {
            var args = new[] { "ADD", "--read", "file.txt" };
            var result = VerbParsingUtility.GetVerbName( args );
            Assert.That( result, Is.EqualTo( "add" ) );
        }

        [TestFixture]
        public class IsMatchingVerbTypeTests
        {
            [Verb( 'a', "add", Aliases = new[] { "create", "destroy", "repair" } )]
            private class MockVerb { }

            [Test]
            public void IsMatchingVerbType_WithMatchingLongName_ShouldReturnTrue()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "add" );
                Assert.That( result, Is.True );
            }

            [Test]
            public void IsMatchingVerbType_WithMatchingShortName_ShouldReturnTrue()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "a" );
                Assert.That( result, Is.True );
            }

            [Test]
            public void IsMatchingVerbType_WithMatchingAlias1_ShouldReturnTrue()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "create" );
                Assert.That( result, Is.True );
            }

            [Test]
            public void IsMatchingVerbType_WithMatchingAlias1_CaseDifferent_ShouldReturnTrue()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "CREATE" );
                Assert.That( result, Is.True );
            }

            [Test]
            public void IsMatchingVerbType_WithMatchingAlias2_ShouldReturnTrue()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "destroy" );
                Assert.That( result, Is.True );
            }

            [Test]
            public void IsMatchingVerbType_WithMatchingAlias3_CaseDifferent_ShouldReturnTrue()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "REPAIR" );
                Assert.That( result, Is.True );
            }

            [Test]
            public void IsMatchingVerbType_WithNonMatchingName_ShouldReturnFalse()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "invalid" );
                Assert.That( result, Is.False );
            }

            [Test]
            public void IsMatchingVerbType_WithDifferentCase_ShouldReturnTrue()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<MockVerb>( "ADD" );
                Assert.That( result, Is.True );
            }

            private class NoVerbAttributeClass { }

            [Test]
            public void IsMatchingVerbType_WithNoVerbAttribute_ShouldReturnFalse()
            {
                var result = VerbParsingUtility.IsMatchingVerbType<NoVerbAttributeClass>( "test" );
                Assert.That( result, Is.False );
            }
        }
    }
}