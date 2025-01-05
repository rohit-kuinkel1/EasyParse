using EasyParser.Core;

namespace EasyParser.Tests
{
    [TestFixture]
    public class SoundexProviderTests
    {
        [Test]
        public void GetCode_NullInput_ReturnsEmptyString()
        {
            string result = SoundexProvider.GetCode( null! );
            Assert.That( result, Is.Empty );
        }

        [Test]
        public void GetCode_EmptyInput_ReturnsEmptyString()
        {
            string result = SoundexProvider.GetCode( "" );
            Assert.That( result, Is.Empty );
        }

        [TestCase( "Robert", "6163" )]
        [TestCase( "Rupert", "6163" )]
        [TestCase( "Rubin", "6150" )]
        [TestCase( "Ashcraft", "0226" )]
        [TestCase( "Ashcroft", "0226" )]
        [TestCase( "Tymczak", "3522" )]
        [TestCase( "Pfister", "1236" )]
        public void GetCode_ValidInput_ReturnsExpectedSoundex( string input, string expected )
        {
            string result = SoundexProvider.GetCode( input );
            Assert.That( result, Is.EqualTo( expected ) );
        }

        [Test]
        public void GetCode_SingleLetter_ReturnsPaddedCode()
        {
            string result = SoundexProvider.GetCode( "A" );
            Assert.That( result, Is.EqualTo( "0000" ) );
        }

        [Test]
        public void GetCode_CaseInsensitive_ReturnsSameCode()
        {
            string upper = SoundexProvider.GetCode( "ROBERT" );
            string lower = SoundexProvider.GetCode( "robert" );
            string mixed = SoundexProvider.GetCode( "RoBeRt" );

            Assert.Multiple( () =>
            {
                Assert.That( upper, Is.EqualTo( lower ) );
                Assert.That( upper, Is.EqualTo( mixed ) );
            } );
        }

        [Test]
        public void GetCode_NonLetterCharacters_IgnoresNonLetters()
        {
            string result = SoundexProvider.GetCode( "O'Brien" );
            Assert.That( result, Is.EqualTo( "0165" ) );
        }

        [Test]
        public void GetCode_ConsecutiveSimilarSounds_IgnoresDuplicates()
        {
            string result = SoundexProvider.GetCode( "Jackson" );
            Assert.That( result, Is.EqualTo( "2250" ) );
        }

        [TestCase( "Williams", "0452" )]
        [TestCase( "Wilson", "0425" )]
        [TestCase( "Woolcock", "0422" )]
        public void GetCode_Names_ReturnsCorrectCode( string input, string expected )
        {
            string result = SoundexProvider.GetCode( input );
            Assert.That( result, Is.EqualTo( expected ) );
        }

        [Test]
        public void GetCode_WhiteSpace_IgnoresWhitespace()
        {
            string result = SoundexProvider.GetCode( "Van Dyke" );
            Assert.That( result, Is.EqualTo( "1532" ) );
        }

        [Test]
        public void GetCode_LongString_ReturnsMaxFourCharacters()
        {
            string result = SoundexProvider.GetCode( "Schwarzenegger" );
            Assert.That( result, Has.Length.EqualTo( 4 ) );
        }

        [Test]
        public void GetCode_NumbersInString_IgnoresNumbers()
        {
            string result = SoundexProvider.GetCode( "R2D2" );
            Assert.That( result, Is.EqualTo( "6300" ) );
        }

        [TestCase( "Smith", "Smythe" )]
        [TestCase( "Peterson", "Pederson" )]
        [TestCase( "Catherine", "Katherene" )]
        public void GetCode_SimilarSoundingNames_ReturnsSameCode( string name1, string name2 )
        {
            string code1 = SoundexProvider.GetCode( name1 );
            string code2 = SoundexProvider.GetCode( name2 );
            Assert.That( code1, Is.EqualTo( code2 ) );
        }
    }
}
