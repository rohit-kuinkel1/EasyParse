using EasyParser.Core;

namespace EasyParser.Tests
{
    [TestFixture]
    public class JaroWinklerSimilarityTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private ISimilarityCheck _similarity;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        /// <summary>
        /// <see cref="_similarity"/> can be instantiated only once since there are no properties in the class itself,
        /// this will make the test a tad bit more efficient as opposed to using <see cref="SetUpAttribute"/>
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _similarity = new JaroWinklerSimilarity();
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
            string source = "MARTHA";
            string target = "MARTHA";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.EqualTo( 1.0 ) );
        }

        [TestCase( "MARTHA", "MARHTA", 0.961 )]
        [TestCase( "DIXON", "DICKSONX", 0.813 )]
        [TestCase( "JELLYFISH", "SMELLYFISH", 0.896 )]
        public void Calculate_SimilarStrings_ReturnsExpectedScore( string source, string target, double expected )
        {
            double result = _similarity.Calculate( source, target );
            //Assert.That(result, Is.EqualTo( expected ) );
            Assert.That( result, Is.InRange( expected - 0.001, expected + 0.001 ) );
        }

        [Test]
        public void Calculate_CompletelyDifferentStrings_ReturnsLowScore()
        {
            string source = "ABC";
            string target = "XYZ";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.EqualTo( 0.0 ) );
        }

        [Test]
        public void Calculate_StringsWithCommonPrefix_ReturnsHigherScore()
        {
            string source = "TRANSPORT";
            string target = "TRANSFER";

            double result = _similarity.Calculate( source, target );
            double resultReversed = _similarity.Calculate( "SPORT", "SFER" );

            Assert.That( result, Is.GreaterThan( resultReversed ), "Strings with common prefix should have had higher similarity" );
        }

        [Test]
        public void Calculate_CaseSensitiveComparison()
        {
            string source = "Martha";
            string target = "MARTHA";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.LessThan( 0.99 ), "Case sensitive comparison should not consider different cased strings as identical" );
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
            string source = "Hello!@#$%";
            string target = "Hello@#$%!";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.GreaterThan( 0.8 ), "Positioning of special characters should have been handled properly" );
        }

        [Test]
        public void Calculate_StringsWithNumbers()
        {
            string source = "Test123";
            string target = "Test321";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.GreaterThan( 0.8 ), "Positioning of numbers should have been handled properly" );
        }

        [Test]
        public void Calculate_StringsWithSpaces()
        {
            string source = "Hello World";
            string target = "Hello  World";

            double result = _similarity.Calculate( source, target );

            Assert.That( result, Is.GreaterThan( 0.9 ), "Difference in spaces should have been handled properly" );
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

            Assert.That( result1 , Is.EqualTo( 0.0 ) );
            Assert.That( result2, Is.EqualTo( 0.0 ) );
            Assert.That( result3, Is.EqualTo( 0.0 ) );
        }



        [TestCase( "MARTHA", "MARHTA" )]
        [TestCase( "DIXON", "DICKSONX" )]
        [TestCase( "JELLYFISH", "SMELLYFISH" )]
        public void Calculate_Symmetry_ReturnsConsistentResults( string str1, string str2 )
        {
            double result1 = _similarity.Calculate( str1, str2 );
            double result2 = _similarity.Calculate( str2, str1 );

            Assert.That( result1, Is.EqualTo( result2 ) );
        }
    }
}
