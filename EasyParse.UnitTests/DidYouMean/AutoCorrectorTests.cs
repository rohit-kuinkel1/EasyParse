using System.Reflection;
using EasyParse.Tests;
using EasyParse.Core;

namespace EasyParse.DidYouMeanTests
{
    [TestFixture]
    public class AutoCorrectorTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private AutoCorrector _autoCorrector;
        private Type _dummyType;
        private PropertyInfo _dummyProperty;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [SetUp]
        public void Setup()
        {
            _autoCorrector = new AutoCorrector();
            _dummyType = typeof( MockClass );
            _dummyProperty = _dummyType.GetProperty( nameof( MockClass.TestProperty ) )!;
        }

        #region Verb Correction Tests
        [Test]
        public void TryCorrectVerb_InvalidVerbWithSuggestions_ReturnsCorrection()
        {
            var availableVerbs = new List<Verb>
            {
                new Verb(_dummyType, new VerbAttribute('c', "command")),
                new Verb(_dummyType, new VerbAttribute('c', "com"))
            };

            var result = _autoCorrector.TryCorrectVerb( "commnad", availableVerbs );

            Assert.Multiple( () =>
            {
                Assert.That( result.Item1, Is.Not.Null );
                Assert.That( ReferenceEquals( result.Item1, availableVerbs[0] ) );
                Assert.That( result.Item2, Is.EqualTo( "command" ) );
            } );
        }

        [Test]
        public void TryCorrectVerb_InvalidVerbNoSuggestions_ReturnsNull()
        {
            var availableVerbs = new List<Verb>
            {
                new Verb(_dummyType, new VerbAttribute('c', "command")),
                new Verb(_dummyType, new VerbAttribute('z', "zoo"))
            };

            var result = _autoCorrector.TryCorrectVerb( "xyz", availableVerbs );

            Assert.Multiple( () =>
            {
                Assert.That( result.Item1, Is.Null );
                Assert.That( result.Item2, Is.Null );
            } );
        }
        #endregion

        #region Option Correction Tests
        [Test]
        public void TryCorrectOption_InvalidOptionWithSuggestions_ReturnsCorrection()
        {
            var availableOptions = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help")),
                new Option(_dummyProperty, new OptionsAttribute('h', "hello")),
                new Option(_dummyProperty, new OptionsAttribute('h', "hell"))
            };

            var result = _autoCorrector.TryCorrectOption( "hlp", availableOptions );

            Assert.Multiple( () =>
            {
                Assert.That( result.Item1, Is.Not.Null );
                Assert.That( ReferenceEquals( result.Item1, availableOptions[0] ) );
                Assert.That( result.Item2, Is.EqualTo( "help" ) );
            } );
        }

        [Test]
        public void TryCorrectOption_InvalidOptionNoSuggestions_ReturnsNull()
        {
            var availableOptions = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help")),
                new Option(_dummyProperty, new OptionsAttribute('h', "hello")),
                new Option(_dummyProperty, new OptionsAttribute('h', "hell"))
            };

            var result = _autoCorrector.TryCorrectOption( "xyz", availableOptions );

            Assert.Multiple( () =>
            {
                Assert.That( result.Item1, Is.Null );
                Assert.That( result.Item2, Is.Null );
            } );
        }
        #endregion

        #region Correction State Tests
        [Test]
        public void HasVerbCorrections_AfterCorrection_ReturnsTrue()
        {
            var availableVerbs = new List<Verb>
            {
                new Verb(_dummyType, new VerbAttribute('c', "command"))
            };

            _autoCorrector.TryCorrectVerb( "commnad", availableVerbs );

            Assert.That( _autoCorrector.HasVerbCorrections, Is.True );
        }

        [Test]
        public void HasOptionCorrections_AfterCorrection_ReturnsTrue()
        {
            var availableOptions = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help"))
            };

            _autoCorrector.TryCorrectOption( "hlp", availableOptions );

            Assert.That( _autoCorrector.HasOptionCorrections, Is.True );
        }

        [Test]
        public void HasAnyCorrections_NoCorrections_ReturnsFalse()
        {
            Assert.That( _autoCorrector.HasAnyCorrections, Is.False );
        }

        [Test]
        public void HasAnyCorrections_AfterCorrections_ReturnsTrue()
        {
            var availableVerbs = new List<Verb>
            {
                new Verb(_dummyType, new VerbAttribute('c', "command"))
            };

            _autoCorrector.TryCorrectVerb( "commnad", availableVerbs );

            Assert.That( _autoCorrector.HasAnyCorrections, Is.True );
        }
        #endregion

        #region Retrieval Methods Tests
        [Test]
        public void GetVerbCorrections_AfterCorrection_ReturnsCorrectMapping()
        {
            var availableVerbs = new List<Verb>
            {
                new Verb(_dummyType, new VerbAttribute('c', "command"))
            };

            _autoCorrector.TryCorrectVerb( "commnad", availableVerbs );
            var corrections = _autoCorrector.GetVerbCorrections();

            Assert.That( corrections["commnad"], Is.EqualTo( "command" ) );
        }

        [Test]
        public void GetOptionCorrections_AfterCorrection_ReturnsCorrectMapping()
        {
            var availableOptions = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help"))
            };

            _autoCorrector.TryCorrectOption( "hlp", availableOptions );
            var corrections = _autoCorrector.GetOptionCorrections();

            Assert.That( corrections["hlp"], Is.EqualTo( "help" ) );
        }

        [Test]
        public void TryCorrectOption_EmptyInput_ReturnsNull()
        {
            var availableOptions = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help"))
            };

            var result = _autoCorrector.TryCorrectOption( string.Empty, availableOptions );

            Assert.Multiple( () =>
            {
                Assert.That( result.Item1, Is.Null );
                Assert.That( result.Item2, Is.Null );
            } );
        }

        [Test]
        public void TryCorrectOption_EmptyCollection_ReturnsNull()
        {
            var availableOptions = new List<Option>
            {
            };

            var result = _autoCorrector.TryCorrectOption( "help", availableOptions );

            Assert.Multiple( () =>
            {
                Assert.That( result.Item1, Is.Null );
                Assert.That( result.Item2, Is.Null );
            } );
        }

        [Test]
        public void GetAllCorrections_AfterCorrections_ReturnsAllMappings()
        {
            var availableVerbs = new List<Verb>
            {
                new Verb(_dummyType, new VerbAttribute('c', "command"))
            };

            var availableOptions = new List<Option>
            {
                new Option(_dummyProperty, new OptionsAttribute('h', "help"))
            };

            _autoCorrector.TryCorrectVerb( "commnad", availableVerbs );
            _autoCorrector.TryCorrectOption( "hlp", availableOptions );

            var allCorrections = _autoCorrector.GetAllCorrections().ToList();

            Assert.Multiple( () =>
            {
                Assert.That( allCorrections, Contains.Item( Tuple.Create( "commnad", "command" ) ) );
                Assert.That( allCorrections, Contains.Item( Tuple.Create( "hlp", "help" ) ) );
            } );
        }
        #endregion
    }
}
