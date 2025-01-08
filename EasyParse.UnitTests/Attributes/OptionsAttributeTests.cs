using EasyParser.Core;

namespace EasyParser.Tests.Core
{
    [TestFixture]
    public class OptionsAttributeTests
    {
        [Test]
        public void Constructor_WithMinimalParameters_SetsBasicProperties()
        {
            const char shortName = 't';
            const string longName = "test";

            var attribute = new OptionsAttribute( shortName, longName );

            Assert.Multiple( () =>
            {
                Assert.That( attribute.ShortName, Is.EqualTo( shortName ) );
                Assert.That( attribute.LongName, Is.EqualTo( longName ) );
                Assert.That( attribute.Required, Is.False, $"{nameof( attribute.Required )} should be false by default" );
                Assert.That( attribute.Default, Is.Null );

                Assert.That( attribute.ErrorMessage, Is.Empty );
                Assert.That( attribute.ErrorMessage, Is.Not.Null, $"{nameof( attribute.ErrorMessage )} should not be null when not provided, rather an empty string" );
                Assert.That( attribute.HelpText, Is.Empty );
                Assert.That( attribute.HelpText, Is.Not.Null, $"{nameof( attribute.HelpText )} should not be null when not provided, rather an empty string" );
            } );
        }

        [Test]
        public void Constructor_WithAllParameters_SetsAllProperties()
        {
            const char shortName = 'f';
            const string longName = "file";
            const bool isRequired = true;
            const string defaultValue = "default.txt";
            const string helpText = "Specify input file";
            const string errorMessage = "Invalid file path";
            string[] aliases = { "input", "source" };

            var attribute = new OptionsAttribute(
                shortName: shortName,
                longName: longName,
                isRequired: isRequired,
                defaultValue: defaultValue,
                helpText: helpText,
                errorMessage: errorMessage,
                aliases: aliases
            );

            Assert.Multiple( () =>
            {
                Assert.That( attribute.ShortName, Is.EqualTo( shortName ) );
                Assert.That( attribute.LongName, Is.EqualTo( longName ) );
                Assert.That( attribute.Required, Is.EqualTo( isRequired ) );
                Assert.That( attribute.Default, Is.EqualTo( defaultValue ) );
                Assert.That( attribute.HelpText, Is.EqualTo( helpText ) );
                Assert.That( attribute.ErrorMessage, Is.EqualTo( errorMessage ) );
                Assert.That( attribute.Aliases, Is.EqualTo( aliases ) );
            } );
        }


        [Test]
        public void Constructor_WithNullDefaultValue_AllowsNull()
        {
            const char shortName = 't';
            const string longName = "test";

            var attribute = new OptionsAttribute(
                shortName,
                longName,
                defaultValue: null
            );

            Assert.That( attribute.Default, Is.Null );
        }

        [Test]
        public void ToString_ReturnsFormattedString()
        {
            const char shortName = 'f';
            const string longName = "file";
            const bool isRequired = true;
            const string defaultValue = "test.txt";
            const string helpText = "Help";
            const string errorMessage = "Error";

            var attribute = new OptionsAttribute(
                shortName: shortName,
                longName: longName,
                isRequired: isRequired,
                defaultValue: defaultValue,
                helpText: helpText,
                errorMessage: errorMessage
            );

            var result = attribute.ToString();

            var expected =
                $"\n\t\tOptionsAttribute: \n" +
                $"\t\t\tLongName:{longName}, \n" +
                $"\t\t\tShortName:{shortName}, \n" +
                $"\t\t\tRequired:{isRequired}, \n" +
                $"\t\t\tDefaultValue:{defaultValue}, \n" +
                $"\t\t\tHelpText:{helpText}, \n" +
                $"\t\t\tErrorMessage:{errorMessage} \n";

            Assert.That( result, Is.EqualTo( expected ), $"{nameof( OptionsAttribute.ToString )} of {nameof( OptionsAttribute )} has been modified from the originally agreed upon state!" );
        }


        [Test]
        public void AttributeUsage_AllowsOnlyOneInstance()
        {
            var attributeType = typeof( OptionsAttribute );

            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
                attributeType, typeof( AttributeUsageAttribute ) )!;

            Assert.That( usage.AllowMultiple, Is.False );
        }

        [Test]
        public void AttributeUsage_OnlyAllowedOnProperties()
        {
            var attributeType = typeof( OptionsAttribute );

            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
                attributeType, typeof( AttributeUsageAttribute ) )!;

            //should only be hung on properties of a class
            Assert.That( usage.ValidOn, Is.EqualTo( AttributeTargets.Property ), $"Only Properties of a given class are allowed to be decorated with {nameof( OptionsAttribute )}!" );
        }

        [TestCase( "" )]
        [TestCase( null )]
        public void Constructor_WithInvalidLongName_ThrowsArgumentException( string invalidLongName )
        {
            const char shortName = 't';
            Assert.Throws<ArgumentNullException>( () => new OptionsAttribute( shortName, invalidLongName ) );
        }

        [TestCase( " " )]
        public void Constructor_WithInvalidLongNameAsWhiteSpaces_ThrowsArgumentException( string invalidLongName )
        {
            const char shortName = 't';
            Assert.Throws<ArgumentException>( () => new OptionsAttribute( shortName, invalidLongName ) );
        }

        [Test]
        public void Constructor_WithDifferentDefaultValueTypes_AcceptsVariousTypes()
        {
            var defaultValues = new object[] { 42, true, "string", 3.14 };
            const char shortName = 't';
            const string longName = "test";

            Assert.Multiple( () =>
            {
                foreach( var defaultValue in defaultValues )
                {
                    Assert.DoesNotThrow( () => new OptionsAttribute( shortName, longName, defaultValue: defaultValue ) );
                }
            } );
        }

        [Test]
        public void Properties_CanBeModifiedAfterConstruction()
        {
            const char shortName = 't';
            const string longName = "test";
            const bool required = true;
            const string errorMessage = "New Error";

            var attribute = new OptionsAttribute( shortName, longName );

            attribute.Required = required;
            attribute.ErrorMessage = errorMessage;
            attribute.Default = 42;

            Assert.Multiple( () =>
            {
                Assert.That( attribute.Required, Is.EqualTo( required ) );
                Assert.That( attribute.ErrorMessage, Is.EqualTo( errorMessage ) );
                Assert.That( attribute.Default, Is.EqualTo( 42 ) );
            } );
        }
    }
}