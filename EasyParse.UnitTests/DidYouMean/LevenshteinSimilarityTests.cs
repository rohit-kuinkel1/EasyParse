using EasyParser.Core;

namespace EasyParser.Tests
{
    [TestFixture]
    public class LevenshteinSimilarityTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private ISimilarityCheck _similarity;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [OneTimeSetUp]
        public void Setup()
        {
            _similarity = new LevenshteinSimilarity();
        }

        [Test]
        public void Calculate_BothStringsEmpty_Returns0()
        {
            string source = "";
            string target = "";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.EqualTo( 0.0 ) );
        }

        [Test]
        public void Calculate_OneStringEmpty_Returns0()
        {
            string source = "test";
            string target = "";

            double result1 = _similarity.Calculate( source, target );
            double result2 = _similarity.Calculate( target, source );

            Assert.That( result1, Is.EqualTo( 0.0 ) );
            Assert.That( result2, Is.EqualTo( 0.0 ) );
        }

        [Test]
        public void Calculate_IdenticalStrings_Returns1()
        {
            string source = "hello";
            string target = "hello";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.EqualTo( 1.0 ) );
        }

        [TestCase( "kitten", "sitting", 0.571, Description = "Classic Levenshtein example" )]
        [TestCase( "saturday", "sunday", 0.625, Description = "Day of week comparison" )]
        [TestCase( "book", "back", 0.5, Description = "Single character substitution" )]
        public void Calculate_SimilarStrings_ReturnsExpectedScore( string source, string target, double expected )
        {
            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.InRange( expected - 0.001, expected + 0.001 ) );
        }

        [Test]
        public void Calculate_SingleCharacterDifference_ReturnsExpectedScore()
        {
            string source = "test";
            string target = "tent";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.EqualTo( 0.75 ), "One character difference in 4 letter word should return 0.75" ); //all 4 match = 100, 3 letters match = 75
        }

        [Test]
        public void Calculate_CompletelyDifferentStrings_ReturnsLowScore()
        {
            string source = "abc";
            string target = "xyz";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.LessThan( 0.4 ) );
        }

        [Test]
        public void Calculate_CaseSensitiveComparison()
        {
            string source = "Test";
            string target = "test";

            double result = _similarity.Calculate( source, target );

            //0.75
            Assert.That( result, Is.LessThan( 0.8 ), "Case sensitive comparison should not consider different cased strings as identical" );
        }

        [Test]
        public void Calculate_LongStrings()
        {
            string source = "ThisIsAVeryLongStringThatWeWillUseForTestingCreatedOnDecember30thOf2024";
            string target = "ThisIsAVeryLongStringThatWeWillUseForTestingCreatedOnDecember30thOf2024";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.EqualTo( 1.0 ) );
        }

        [Test]
        public void Calculate_StringsWithSpecialCharacters()
        {
            string source = "test!@#";
            string target = "test#@!";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.GreaterThan( 0.5 ), "Positioning of special characters should have been handled properly" );
        }

        [Test]
        public void Calculate_StringsWithNumbers()
        {
            string source = "test123";
            string target = "test321";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.GreaterThan( 0.5 ), "Positioning of numbers should have been handled properly" );
        }

        [Test]
        public void Calculate_StringsWithSpaces()
        {
            string source = "Hello world";
            string target = "Hello  world";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.GreaterThan( 0.8 ), "Number of spaces should have been handled properly" );
        }

        [Test]
        public void Calculate_NullStrings_DoesNotThrowAndReturnsZero()
        {
            double result1 = 0.0;
            double result2 = 0.0;
            double result3 = 0.0;

            Assert.DoesNotThrow( () =>
            {
                result1 = _similarity.Calculate( null!, "test" );
                result2 = _similarity.Calculate( "test", null! );
                result3 = _similarity.Calculate( null!, null! );
            } );

            Assert.That( 0.0, Is.EqualTo( result1 ) );
            Assert.That( 0.0, Is.EqualTo( result2 ) );
            Assert.That( 0.0, Is.EqualTo( result3 ) );
        }

        [TestCase( "kitten", "sitting" )]
        [TestCase( "book", "back" )]
        [TestCase( "hello", "hallo" )]
        public void Calculate_Symmetry_ReturnsConsistentResults( string str1, string str2 )
        {
            double result1 = _similarity.Calculate( str1, str2 );
            double result2 = _similarity.Calculate( str2, str1 );

            Assert.That( result1, Is.EqualTo( result2 ) );
        }

        [Test]
        public void Calculate_SubstringRelationship()
        {
            string source = "test";
            string target = "testing";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.GreaterThan( 0.5 ), "Substring should have had relatively high similarity" );
        }

        [Test]
        public void Calculate_SingleCharacterStrings()
        {
            string source = "a";
            string target = "b";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.EqualTo( 0.0 ), "Different single characters should have had zero similarity" );
        }
    }
}
