using System.Text.RegularExpressions;
using EasyParser.Core;
using EasyParser.Utility;

namespace EasyParser.Tests.Core
{
    [TestFixture]
    public class SettingsAttributeTests
    {
        [Test]
        public void Constructor_WithDefaultValues_SetsPropertiesCorrectly()
        {
            var attribute = new SettingsAttribute();

            Assert.Multiple( () =>
            {
                Assert.That( attribute.MinValue, Is.EqualTo( SettingsAttribute.DefaultNotProvidedMinMax ) );
                Assert.That( attribute.MaxValue, Is.EqualTo( SettingsAttribute.DefaultNotProvidedMinMax ) );
                Assert.That( attribute.RegexPattern, Is.Empty );
                Assert.That( attribute.RegexOnFailureMessage, Is.EqualTo( "Validation against the provided Regex Pattern failed" ) );
                Assert.That( attribute.AllowedValues, Is.Null );
                Assert.That( attribute.CompiledRegex, Is.Null );
            } );
        }

        [Test]
        public void Constructor_WithAllParameters_SetsAllPropertiesCorrectly()
        {
            const int minValue = 1;
            const int maxValue = 10;
            const string pattern = @"^\d+$";
            const string errorMessage = "Numbers only";
            var allowedValues = new object[] { 1, 2, 3 };

            var attribute = new SettingsAttribute(
                minValue: minValue,
                maxValue: maxValue,
                regexPattern: pattern,
                regexErrorMessage: errorMessage,
                allowedValues: allowedValues
            );

            Assert.Multiple( () =>
            {
                Assert.That( attribute.MinValue, Is.EqualTo( minValue ) );
                Assert.That( attribute.MaxValue, Is.EqualTo( maxValue ) );
                Assert.That( attribute.RegexPattern, Is.EqualTo( pattern ) );
                Assert.That( attribute.RegexOnFailureMessage, Is.EqualTo( errorMessage ) );
                Assert.That( attribute.AllowedValues, Is.EqualTo( allowedValues ) );
                Assert.That( attribute.CompiledRegex, Is.Not.Null );
            } );
        }

        [Test]
        public void Constructor_WithValidRegexPattern_CompilesRegex()
        {
            const string pattern = @"^[a-z]+$";

            var attribute = new SettingsAttribute( regexPattern: pattern );

            Assert.Multiple( () =>
            {
                Assert.That( attribute.CompiledRegex, Is.Not.Null );
                Assert.That( attribute.CompiledRegex?.IsMatch( "test" ), Is.True );
                Assert.That( attribute.CompiledRegex?.IsMatch( "123" ), Is.False );
            } );
        }

        [Test]
        public void Constructor_WithEmptyRegexPattern_DoesNotCompileRegex()
        {
            var attribute = new SettingsAttribute( regexPattern: "" );

            Assert.That( attribute.CompiledRegex, Is.Null );
        }

        [Test]
        public void Constructor_WithWhitespaceRegexPattern_DoesNotCompileRegex()
        {
            Assert.Multiple( () =>
            {
                //invalid regex patterns like whitespaces are just ignored
                Assert.DoesNotThrow( () => new SettingsAttribute( regexPattern: "   " ) );
            } );
        }

        [Test]
        public void Constructor_WithInvalidRegexPattern_ThrowsArgumentException()
        {
            const string invalidPattern = "[";

            Assert.Throws<RegexParseException>( () => new SettingsAttribute( regexPattern: invalidPattern ) );
        }

        [Test]
        public void AttributeUsage_AllowsOnlyOneInstance()
        {
            var attributeType = typeof( SettingsAttribute );

            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
                attributeType, typeof( AttributeUsageAttribute ) )!;

            //one option gets only one settings attribute
            Assert.That( usage.AllowMultiple, Is.False, $"Only One {nameof(SettingsAttribute)} per class property is allowed!" );
        }

        [Test]
        public void AttributeUsage_OnlyAllowedOnProperties()
        {
            var attributeType = typeof( SettingsAttribute );

            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
                attributeType, typeof( AttributeUsageAttribute ) )!;

            Assert.That( usage.ValidOn, Is.EqualTo( AttributeTargets.Property ) );
        }

        [Test]
        public void Constructor_WithAllowedValues_AcceptsDifferentTypes()
        {
            var allowedValues = new object[] { 1, "string", true, 3.14 };

            var attribute = new SettingsAttribute( allowedValues: allowedValues );

            Assert.That( attribute.AllowedValues, Is.EqualTo( allowedValues ) );
        }

        [Test]
        public void Constructor_WithChangeInAllowedValues_ChangeIsNotReflected()
        {
            var allowedValues = new object[] { 1, "string", true, 3.14 };

            var attribute = new SettingsAttribute( allowedValues: allowedValues );
            Assert.That( attribute.AllowedValues, Is.EqualTo( allowedValues ) );

            allowedValues = new object[] { 2, "stringstring", true, 6.28 };
            Assert.That( attribute.AllowedValues, Is.Not.EqualTo( allowedValues ) );

            const int newVal = 4;
            allowedValues[0] = newVal;
#pragma warning disable CS8602
            Assert.That( attribute.AllowedValues[0], Is.Not.EqualTo( newVal ) );
#pragma warning restore CS8602
        }

        [Test]
        public void Constructor_WithMinValueGreaterThanMaxValue_ThrowsIllegalOperationException()
        {
            Assert.Throws<IllegalOperationException>( () => new SettingsAttribute( minValue: 10, maxValue: 1 ) );
        }

        [Test]
        public void Properties_CanBeModifiedAfterConstruction()
        {
            var attribute = new SettingsAttribute();
            const int minVal = 5;
            const int maxVal = 10;
            const string regexPattern = @"^\d+$";
            const string regexOnFailureMessage = "Numbers only";
            object[] allowedVals = new object[] { 1, 2, 3 };

            attribute.MinValue = minVal;
            attribute.MaxValue = maxVal;
            attribute.RegexPattern = regexPattern;
            attribute.RegexOnFailureMessage = regexOnFailureMessage;
            attribute.AllowedValues = new object[] { 1, 2, 3 };

            Assert.Multiple( () =>
            {
                Assert.That( attribute.MinValue, Is.EqualTo( minVal ) );
                Assert.That( attribute.MaxValue, Is.EqualTo( maxVal ) );
                Assert.That( attribute.RegexPattern, Is.EqualTo( regexPattern ) );
                Assert.That( attribute.RegexOnFailureMessage, Is.EqualTo( regexOnFailureMessage ) );
                Assert.That( attribute.AllowedValues, Is.EqualTo( allowedVals ) );
            } );
        }
    }
}