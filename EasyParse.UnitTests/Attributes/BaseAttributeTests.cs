using EasyParse.Core;
namespace EasyParse.AttributeTests
{
    /// <remarks>
    /// testing <see cref="Attribute"/> is not standard but in our case <see cref="BaseAttribute"/> has a logic in <see cref="BaseAttribute.Aliases"/> so it makes sense to test
    /// that logic
    /// </remarks>
    [TestFixture]
    public class BaseAttributeTests
    {
        private class MockAttribClassAttribute : BaseAttribute
        {
            public MockAttribClassAttribute( string helpText, string errorMessage = "", params string[] aliases )
                : base( helpText, errorMessage, aliases )
            {
            }
        }

        [Test]
        public void Constructor_SetsHelpText()
        {
            var attribute = new MockAttribClassAttribute( "Help message" );

            Assert.That( attribute.HelpText, Is.EqualTo( "Help message" ) );
        }

        [Test]
        public void Aliases_WhenEmpty_ReturnsEmptyArray()
        {
            var attribute = new MockAttribClassAttribute( "Help message" );

            Assert.That( attribute.Aliases, Is.Empty );
        }

        [Test]
        public void Aliases_WithValidAliases_SetsAllValidAliases()
        {
            var attribute = new MockAttribClassAttribute( "Help message" );
            var validAliases = new[] { "reading", "writing", "studying" };

            attribute.Aliases = validAliases;

            Assert.That( attribute.Aliases, Has.Length.EqualTo( validAliases.Length ) );
            Assert.That( attribute.Aliases, Is.EquivalentTo( validAliases ) );
        }

        [Test]
        public void Aliases_WithMixedAliases_OnlyKeepsValidOnes()
        {
            var attribute = new MockAttribClassAttribute( "Help message" );
            var mixedAliases = new[] { "reading", "s", "studying", "" };

            attribute.Aliases = mixedAliases;

            //because 2 aliases didnt fulfill the criteria of Length >=2, see BaseAttribute.Aliases
            var expectedAliases = new[] { "reading", "studying" };

            Assert.Multiple( () =>
            {
                Assert.That( mixedAliases, Has.Length.Not.EqualTo( attribute.Aliases.Length ) );
                Assert.That( expectedAliases, Has.Length.EqualTo( 2 ) );
                Assert.That( attribute.Aliases, Is.EquivalentTo( expectedAliases ) );
            } );
        }

        [Test]
        public void Aliases_WithAllInvalidAliases_ReturnsEmptyArray()
        {
            var attribute = new MockAttribClassAttribute( "Help message" );

            var invalidAliases = new[] { "s", "r", "" };

            attribute.Aliases = invalidAliases;

            //because all current aliases didnt fulfill the criteria of Length >=2, see BaseAttribute.Aliases
            Assert.That( attribute.Aliases, Is.Empty );

        }

        //This will move in the direction of integration testing
        //[Test]
        //public void Aliases_WithAllInvalidAliases_MessageDisplayedInConsole()
        //{
        //    Logger.Initialize(LogLevel.Debug);
        //    var attribute = new MockAttribClassAttribute( "Help message" );

        //    var invalidAliases = new[] { "s", "r", "" };

        //    var debugOutput = new StringWriter();
        //    Console.SetOut( debugOutput );
        //    attribute.Aliases = invalidAliases;
        //    var logMessage = debugOutput.ToString();
        //    Assert.That( logMessage, Does.Contain( "Some aliases were discarded because they were either empty or their length was less than the defined threshold" ) );

        //    Logger.Reset();
        //}

        [Test]
        public void Aliases_WithSpacesAsAliases_ReturnsEmptyArray()
        {
            var attribute = new MockAttribClassAttribute( "Help message" );
            //spaces are not valid aliases
            var invalidAliases = new[] { "", "  ", "   " };

            attribute.Aliases = invalidAliases;

            //because all current aliases didnt fulfill the criteria of Length >=2, see BaseAttribute.Aliases
            Assert.That( attribute.Aliases, Is.Empty );
            Assert.Multiple( () =>
            {
                Assert.That( attribute.Aliases, Does.Not.Contain( invalidAliases[1] ) );
                Assert.That( attribute.Aliases, Does.Not.Contain( invalidAliases[2] ) );
            } );
        }

        [Test]
        public void Constructor_WithAliases_SetsValidAliases()
        {
            var attribute = new MockAttribClassAttribute( "Help message", "ERROR", "reading", "s", "studying" );

            var expectedAliases = new[] { "reading", "studying" };
            Assert.That( attribute.Aliases, Is.EquivalentTo( expectedAliases ) );
        }
    }
}