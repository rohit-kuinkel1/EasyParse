using System.Reflection;
using EasyParse.Tests;
using EasyParser.Core;

namespace EasyParser.Tests
{

    [TestFixture]
    public class DidYouMeanTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private DidYouMean _didYouMean;
        private Type _dummyType;
        private PropertyInfo _dummyProperty;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [SetUp]
        public void Setup()
        {
            _didYouMean = new DidYouMean();
            _dummyType = typeof( MockClass );
            _dummyProperty = _dummyType.GetProperty( nameof( MockClass.TestProperty ) )!;
        }

        #region Verb Suggestion Tests
        [Test]
        public void GetVerbSuggestions_EmptyInput_ReturnsEmptyList()
        {
            var verbAttr = new VerbAttribute( 'c', "command" );
            var verb = new Verb( _dummyType, verbAttr );
            var suggestions = _didYouMean.GetVerbSuggestions( "", verb );

            Assert.That( suggestions, Is.Empty );
        }

        [Test]
        public void GetVerbSuggestions_NullInput_ReturnsEmptyList()
        {
            var verbAttr = new VerbAttribute( 'c', "command" );
            var verb = new Verb( _dummyType, verbAttr );
            var suggestions = _didYouMean.GetVerbSuggestions( null!, verb );

            Assert.That( suggestions, Is.Empty );
        }

        [Test]
        public void GetVerbSuggestions_WithShortAndLongNames_ReturnsBothOptions()
        {
            var verbAttr = new VerbAttribute( 'c', "command" );
            var verb = new Verb( _dummyType, verbAttr );

            var suggestions = _didYouMean.GetVerbSuggestions( "commnad", verb ).ToList();

            Assert.That( suggestions, Contains.Item( "command" ) );
            Assert.That( suggestions, Contains.Item( "c" ) );
        }

        [Test]
        public void GetVerbSuggestions_SimilarInput_ReturnsSuggestions()
        {
            var verb = new Verb( _dummyType, new VerbAttribute( 'c', "command" ) );
            var suggestions = _didYouMean.GetVerbSuggestions( "comand", verb ).ToList();

            Assert.That( suggestions, Contains.Item( "command" ) );
        }

        [Test]
        public void GetVerbSuggestions_SameInput_ReturnsSuggestions()
        {
            var verb = new Verb( _dummyType, new VerbAttribute( 'c', "command" ) );
            var suggestions = _didYouMean.GetVerbSuggestions( "command", verb ).ToList();

            Assert.That( suggestions, Contains.Item( "command" ) );
        }

        [Test]
        public void GetVerbSuggestions_CaseInsensitive_ReturnsSuggestions()
        {
            var verb = new Verb( _dummyType, new VerbAttribute( 'c', "command" ) );
            var suggestions = _didYouMean.GetVerbSuggestions( "COMMAND", verb ).ToList();

            Assert.That( suggestions, Contains.Item( "command" ) );
        }

        [Test]
        public void GetVerbSuggestions_CaseInsensitiveMisspelled_ReturnsSuggestions()
        {
            var verb = new Verb( _dummyType, new VerbAttribute( 'c', "command" ) );
            var suggestions = _didYouMean.GetVerbSuggestions( "COMMNDA", verb ).ToList();

            Assert.That( suggestions, Contains.Item( "command" ) );
        }

        [Test]
        public void GetVerbSuggestions_VeryDifferentInput_ReturnsNoSuggestions()
        {
            var verb = new Verb( _dummyType, new VerbAttribute( 'c', "command" ) );
            var suggestions = _didYouMean.GetVerbSuggestions( "xyz", verb ).ToList();

            Assert.That( suggestions, Is.Empty );
        }

        [Test]
        public void GetVerbSuggestions_SingleCharacterInput_HandlesProperly()
        {
            var verb = new Verb( _dummyType, new VerbAttribute( 'c', "command" ) );
            var suggestions = _didYouMean.GetVerbSuggestions( "c", verb ).ToList();

            Assert.That( suggestions, Contains.Item( "c" ) );
        }

        [Test]
        public void GetVerbSuggestions_WhitespaceInput_ReturnsEmptyList()
        {
            var verb = new Verb( _dummyType, new VerbAttribute( 'c', "command" ) );
            var suggestions = _didYouMean.GetVerbSuggestions( "   ", verb ).ToList();

            Assert.That( suggestions, Is.Empty );
        }

        #endregion

        #region Option Suggestion Tests
        [Test]
        public void GetOptionSuggestions_EmptyInput_ReturnsEmptyList()
        {
            var options = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help"))
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "", options );
            Assert.That( suggestions, Is.Empty );
        }

        [Test]
        public void GetOptionSuggestions_WithAliases_ReturnsAllPossibleMatches()
        {
            var optionsAttr = new OptionsAttribute( 'h', "help" )
            {
                Aliases = new[] { "??", "info", "howto" }
            };

            var options = new List<Option>
            {
                new Option(_dummyProperty, optionsAttr)
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "hlp", options ).ToList();

            Assert.Multiple( () =>
            {
                Assert.That( suggestions, Contains.Item( "help" ) );
                Assert.That( suggestions, Contains.Item( "h" ) );
                Assert.That( suggestions, Contains.Item( "??" ) );
                Assert.That( suggestions, Contains.Item( "info" ) );
                Assert.That( suggestions, Contains.Item( "howto" ) );
            } );
        }

        [Test]
        public void GetOptionSuggestions_PrefixMatch_ReturnsHigherPriority()
        {
            var options = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h',"help")),
                new Option(_dummyProperty, new OptionsAttribute('h', "helicopter"))
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "hel", options ).ToList();

            Assert.That( suggestions, Contains.Item( "h" ) );
            Assert.That( suggestions.First(), Is.EqualTo( "help" ) );
            Assert.That( suggestions, Contains.Item( "helicopter" ) );
        }

        [Test]
        public void GetOptionSuggestions_SoundexMatch_IncludesPhoneticallySimilarWords()
        {
            var options = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('f',"filter")),
                new Option(_dummyProperty, new OptionsAttribute('f',"folder"))
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "fildr", options ).ToList();

            Assert.That( suggestions, Contains.Item( "f" ) );
            Assert.That( suggestions, Contains.Item( "filter" ) );
            Assert.That( suggestions, Contains.Item( "folder" ) );
        }

        [Test]
        public void GetOptionSuggestions_EmptyOptionsList_ReturnsEmptyList()
        {
            var suggestions = _didYouMean.GetOptionSuggestions( "help", new List<Option>() );
            Assert.That( suggestions, Is.Empty );
        }

        [Test]
        public void GetOptionSuggestions_MultipleOptionsWithSimilarNames_ReturnsAllMatches()
        {
            var options = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('s', "save")),
                new Option(_dummyProperty, new OptionsAttribute('v', "save-as")),
                new Option(_dummyProperty, new OptionsAttribute('a', "save-all"))
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "sav", options ).ToList();

            Assert.Multiple( () =>
            {
                Assert.That( suggestions, Contains.Item( "save" ) );
                Assert.That( suggestions, Contains.Item( "save-as" ) );
                Assert.That( suggestions, Contains.Item( "save-all" ) );
            } );
        }

        [Test]
        public void GetOptionSuggestions_CaseInsensitiveMatch_ReturnsSuggestions()
        {
            var options = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help"))
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "HELP", options ).ToList();
            Assert.That( suggestions, Contains.Item( "help" ) );
        }

        [Test]
        public void GetOptionSuggestions_PartialMatch_ReturnsRelevantSuggestions()
        {
            var options = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('d', "debug")),
                new Option(_dummyProperty, new OptionsAttribute('v', "verbose"))
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "deb", options ).ToList();
            Assert.That( suggestions, Contains.Item( "debug" ) );
            Assert.That( suggestions, Does.Not.Contain( "verbose" ) );
        }

        [Test]
        public void GetOptionSuggestions_ComplexAliases_HandlesCorrectly()
        {
            var optionsAttr = new OptionsAttribute( 'c', "config" )
            {
                Aliases = new[] { "configuration", "conf", "cfg" }
            };

            var options = new List<Option>
            {
                new Option(_dummyProperty, optionsAttr)
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "conf", options ).ToList();

            Assert.Multiple( () =>
            {
                Assert.That( suggestions, Contains.Item( "config" ) );
                Assert.That( suggestions, Contains.Item( "configuration" ) );
                Assert.That( suggestions, Contains.Item( "conf" ) );
                Assert.That( suggestions, Contains.Item( "cfg" ) );
                Assert.That( suggestions, Contains.Item( "c" ) );
            } );
        }

        [Test]
        public void GetOptionSuggestions_NoSimilarOptions_ReturnsEmptyList()
        {
            var options = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help")),
                new Option(_dummyProperty, new OptionsAttribute('v', "version"))
            };

            var suggestions = _didYouMean.GetOptionSuggestions( "xyz", options ).ToList();
            Assert.That( suggestions, Is.Empty );
        }
        #endregion
    }
}