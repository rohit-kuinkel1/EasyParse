namespace EasyParser.MiscTests
{
    [TestFixture]
    public class LoggerTests
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private string _testLogDirectory;
        private StringWriter _consoleOutput;
        private TextWriter _originalConsoleOut;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testLogDirectory = Path.Combine( Path.GetTempPath(), "EasyParserTestLogs" );
            if( Directory.Exists( _testLogDirectory ) )
            {
                Directory.Delete( _testLogDirectory, recursive: true );
            }
        }

        [SetUp]
        public void Setup()
        {
            Logger.IsLoggerEnabled = true;

            _originalConsoleOut = Console.Out;
            _consoleOutput = new StringWriter();
            Console.SetOut( _consoleOutput );

            if( !Directory.Exists( _testLogDirectory ) )
            {
                Directory.CreateDirectory( _testLogDirectory );
            }
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut( _originalConsoleOut );
            _consoleOutput.Dispose();

            //if( Directory.Exists( _testLogDirectory ) )
            //{
            //    Directory.Delete( _testLogDirectory, recursive: true );
            //}
        }

        [Test]
        public void Logger_WhenDisabled_ShouldNotLog()
        {
            Logger.Initialize();
            Logger.IsLoggerEnabled = false;

            Logger.Info( "This should not be logged" );

            Assert.Multiple( () =>
            {
                Assert.That( _consoleOutput.ToString(), Is.Not.Empty );
                Assert.That( _consoleOutput.ToString(), Does.Not.Contain( "INFO" ) );
            } );
        }

        [Test]
        public void Logger_WhenMessageBelowMinLogLevel_ShouldNotLog()
        {
            Logger.Initialize( LogLevel.Error );

            Logger.Info( "This should not be logged" );

            string output = _consoleOutput.ToString();

            Assert.Multiple( () =>
            {
                Assert.That( output, Does.Not.Contain( "ERROR" ) );
                Assert.That( output, Is.Not.Empty );
            } );

        }

        [Test]
        public void Logger_WithNullMessage_ShouldHandleGracefully()
        {
            Logger.MinLogLevel = LogLevel.Debug;
            Logger.Info( null );

            string output = _consoleOutput.ToString();

            Assert.Multiple( () =>
            {
                Assert.That( output, Does.Contain( "INFO" ) );
                Assert.That( output, Does.Not.Contain( "null" ) );
                Assert.That( _consoleOutput.ToString(), Is.Not.Empty );
            } );
        }

        [Test]
        public void Logger_WithEmptyMessage_ShouldHandleGracefully()
        {
            Logger.MinLogLevel = LogLevel.Debug;
            Logger.Info( "  " );

            string output = _consoleOutput.ToString();
            Assert.Multiple( () =>
            {
                Assert.That( _consoleOutput.ToString(), Is.Not.Empty );
                Assert.That( output, Does.Contain( "INFO" ) );
            } );

        }

        [Test]
        public void Initialize_WithBackTraceLevel_ShouldDefaultToDebug()
        {
            Logger.Initialize( LogLevel.Debug );

            Logger.BackTrace( "This should not be logged" );
            Logger.Debug( "This should be logged" );

            string output = _consoleOutput.ToString();

            Assert.Multiple( () =>
            {
                Assert.That( output, Does.Not.Contain( "BACKTRACE" ) );
                Assert.That( output, Does.Contain( "DEBUG" ) );
            } );
        }

        [Test]
        public void Logger_RedirectToFile_ShouldCreateLogFile()
        {
            Logger.Initialize(
                minLogLevel: LogLevel.Info,
                redirectLogsToFile: true,
                baseLogDirectory: _testLogDirectory
            );

            //TestContext.Out.WriteLine( _testLogDirectory );
            Logger.Info( "Test log message" );

            //get all the files from this dir EasyParserTestLogs\EasyParserLogs
            string fullPath = Path.Combine( _testLogDirectory, Logger.EasyParseLogDir );

            string[] logFiles = Directory.GetFiles( fullPath );
            string logContent = File.ReadAllText( logFiles[0] );

            Assert.Multiple( () =>
            {
                Assert.That( logFiles.Length, Is.EqualTo( 1 ) );
                Assert.That( logContent, Does.Contain( "Test log message" ) );
            } );
        }

        [Test]
        public void Logger_RedirectToFile_WithInvalidPath_ShouldHandleGracefully()
        {
            var nonExistingPath = @"NonExistentDirectory";
            //TestContext.Out.WriteLine( $"Path was: {nonExistingPath}" );

            Logger.Initialize(
                minLogLevel: LogLevel.Info,
                redirectLogsToFile: true,
                baseLogDirectory: nonExistingPath
            );


            Assert.DoesNotThrow( () => Logger.Info( "Test message" ) );
        }

        [Test]
        public void Logger_RedirectToFile_WithInvalidPathIncludingDir_ShouldHandleGracefully()
        {
            var nonExistingPath = @"ZNonExistentDirectory";
            //TestContext.Out.WriteLine( $"Path was: {nonExistingPath}" );

            Logger.Initialize(
                minLogLevel: LogLevel.Info,
                redirectLogsToFile: true,
                baseLogDirectory: nonExistingPath
            );

            Assert.DoesNotThrow( () => Logger.Info( "Test message" ) );
        }

        [TestCase( LogLevel.Debug )]
        [TestCase( LogLevel.Info )]
        [TestCase( LogLevel.Warning )]
        [TestCase( LogLevel.Error )]
        [TestCase( LogLevel.Critical )]
        public void Logger_DifferentLevels_ShouldLogCorrectly( LogLevel level )
        {
            Logger.Initialize( level );

            Logger.Log( level, $"Test {level} message" );

            string output = _consoleOutput.ToString();

            Assert.Multiple( () =>
            {
                Assert.That( output.Contains( level.ToString(), StringComparison.OrdinalIgnoreCase ), Is.True );
                Assert.That( output, Does.Contain( $"Test {level} message" ) );
            } );
        }

        [Test]
        public void Logger_LongMessage_ShouldHandleGracefully()
        {
            string longMessage = new string( 'a', 1000 );

            Assert.DoesNotThrow( () => Logger.Info( longMessage ) );
        }

        [Test]
        public void Logger_SpecialCharacters_ShouldHandleGracefully()
        {
            string specialChars = "!@#$%^&*()_+\n\t\r";
            Logger.Initialize();
            Logger.Info( specialChars );

            string output = _consoleOutput.ToString();
            Assert.Multiple( () =>
            {
                Assert.That( output, Does.Contain( specialChars ) );
                Assert.That( output, Does.Contain( "INFO" ) );
            } );
        }
    }
}