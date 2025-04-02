using System.Runtime.Versioning;
using System.Security.AccessControl;
using EasyParse.Misc;

namespace EasyParse.MiscTests
{
    [SupportedOSPlatform( "windows" )]
    [TestFixture]
    public class FileHandlerTests
    {
        private string _testDirectory = string.Empty; //tmp val to stop the compiler from complaining

        [SetUp]
        public void Setup()
        {
            _testDirectory = Path.Combine( Path.GetTempPath(), "FileHandlerTests_" + Guid.NewGuid().ToString() );
            Directory.CreateDirectory( _testDirectory );

            //set the dir as a normal directory type
            var dirInfo = new DirectoryInfo( _testDirectory );
            var security = dirInfo.GetAccessControl();
            security.AddAccessRule( new FileSystemAccessRule(
                "Everyone",
                FileSystemRights.Write | FileSystemRights.CreateFiles,
                AccessControlType.Allow
            ) );
            dirInfo.SetAccessControl( security );
        }

        [TearDown]
        public void TearDown()
        {
            if( Directory.Exists( _testDirectory ) )
            {
                Directory.Delete( _testDirectory, recursive: true );
            }
        }

        [Test]
        public void GetInitialDirectory_ShouldReturnValidDirectory()
        {
            var result = FileHandler.GetInitialDirectory();
            TestContext.Out.WriteLine( result );
            Assert.Multiple( () =>
            {
                Assert.That( result, Is.Not.Null );
                Assert.That( result?.EndsWith( @"EasyParse\EasyParse\EasyParse.UnitTests\bin\Release\net8.0" ) ?? false, Is.True, "Did you change from Release to Debug mode ?" );
                Assert.That( Directory.Exists( result ), Is.True );
            } );
        }

        [Test]
        public void TestDirectoryWriteAccess_WithWritableDirectory_ReturnsTrue()
        {
            var directory = _testDirectory;

            var result = FileHandler.TestDirectoryWriteAccess( directory );

            //because we set the FileAttributes to Normal in Setup, this should be true here
            Assert.That( result, Is.True );
        }

        [Test]
        public void TestDirectoryWriteAccess_WithReadOnlyDirectory_ReturnsFalse()
        {
            var readOnlyDir = Path.Combine( _testDirectory, "readonlyDir" );
            Directory.CreateDirectory( readOnlyDir );

            var dirInfo = new DirectoryInfo( readOnlyDir );
            var security = dirInfo.GetAccessControl();
            security.AddAccessRule( new FileSystemAccessRule(
                "Everyone",
                FileSystemRights.Write | FileSystemRights.CreateFiles,
                AccessControlType.Deny
            ) );
            dirInfo.SetAccessControl( security );

            var result = FileHandler.TestDirectoryWriteAccess( readOnlyDir );

            //since we just set the access control type to deny, directory readonly should not be writable
            Assert.That( result, Is.False );
        }

        [Test]
        public void FindWritableParentDirectory_WithNullDirectory_ReturnsNull()
        {
            var result = FileHandler.FindWritableParentDirectory( null );

            Assert.That( result, Is.Null );
        }

        [Test]
        public void FindWritableParentDirectory_WithValidDirectory_ReturnsWritableDirectory()
        {
            var startDir = Path.Combine( _testDirectory, "level1", "level2" );
            Directory.CreateDirectory( startDir );

            // AppData\Local\Temp
            var result = FileHandler.FindWritableParentDirectory( startDir );

            Assert.Multiple( () =>
            {
                Assert.That( result, Is.Not.Null );
#pragma warning disable CS8604
                Assert.That( FileHandler.TestDirectoryWriteAccess( result ), Is.True );
#pragma warning restore CS8604
            } );
        }

        [Test]
        public void SaveConfigFile_WithValidDirectory_SavesFile()
        {
            const string configContent = "test config content 1234 created on 6th january 2025";

            FileHandler.SaveConfigFile( _testDirectory, configContent );

            var filePath = Path.Combine( _testDirectory, Template.templateFileName );
            Assert.Multiple( () =>
            {
                Assert.That( File.Exists( filePath ), Is.True );
                Assert.That( File.ReadAllText( filePath ), Is.EqualTo( configContent ) );
            } );

            File.Delete( filePath ); //dont really need to do this since teardown deletes _testDirectory anyways but lets keep it here
        }

        [Test]
        public void SaveConfigFile_WithNullDirectory_SavesInCurrentDirectoryWithoutThrowingException()
        {
            const string configContent = "test config content 1234 created on 6th january 2025";

            Assert.DoesNotThrow( () => FileHandler.SaveConfigFile( null, configContent ) );

            //if I pass a directory thats not existing to SaveConfigFile, it should fall back to Directory.GetCurrentDirectory()
            var filePath = Path.Combine( Directory.GetCurrentDirectory(), Template.templateFileName );
            Assert.Multiple( () =>
            {
                Assert.That( File.Exists( filePath ), Is.True );
                Assert.That( File.ReadAllText( filePath ), Is.EqualTo( configContent ) );
            } );

            File.Delete( filePath ); //dont really need to do this since teardown deletes _testDirectory anyways but lets keep it here
        }

        [Test]
        public void SaveConfigFile_WithInvalidDirectory_SavesInCurrentDirectoryWithoutThrowingException()
        {
            const string configContent = "test config content 1234 created on 6th january 2025";

            Assert.DoesNotThrow( () => FileHandler.SaveConfigFile( "Z:\\NonExistentDirectory6thJan2025", configContent ) );

            var filePath = Path.Combine( Directory.GetCurrentDirectory(), Template.templateFileName );

            Assert.Multiple( () =>
            {
                Assert.That( File.Exists( filePath ), Is.True );
                Assert.That( File.ReadAllText( filePath ), Is.EqualTo( configContent ) );
            } );

            File.Delete( filePath );  //dont really need to do this since teardown deletes _testDirectory anyways but lets keep it here
        }

        [Test]
        public void SaveConfigFile_WithEmptyContent_SavesEmptyFile()
        {
            const string emptyContent = "";

            FileHandler.SaveConfigFile( _testDirectory, emptyContent );

            var filePath = Path.Combine( _testDirectory, Template.templateFileName );
            Assert.Multiple( () =>
            {
                Assert.That( File.Exists( filePath ), Is.True );
                Assert.That( File.ReadAllText( filePath ), Is.EqualTo( emptyContent ) );
            } );

            File.Delete( filePath );  //dont really need to do this since teardown deletes _testDirectory anyways but lets keep it here
        }

        [Test]
        public void SaveConfigFile_WithLongContent_SavesSuccessfully()
        {
            var longContent = new string( 'a', 1000000 ); // 1 million char of a

            FileHandler.SaveConfigFile( _testDirectory, longContent );

            var filePath = Path.Combine( _testDirectory, Template.templateFileName );
            Assert.Multiple( () =>
            {
                Assert.That( File.Exists( filePath ), Is.True );
                Assert.That( File.ReadAllText( filePath ), Is.EqualTo( longContent ) );
            } );

            File.Delete( filePath ); //dont really need to do this since teardown deletes _testDirectory anyways but lets keep it here
        }

        [Test]
        public void SaveConfigFile_WithInvalidDirectoryName_DoesNotThrowException()
        {
            const string configContent = "test config content 1234 created on 6th january 2025";
            var invalidDirectory = "invalid<>:\"/\\|?*directory";

            Assert.DoesNotThrow( () => FileHandler.SaveConfigFile( invalidDirectory, configContent ) );
        }

        [Test]
        public void SaveConfigFile_WithUnicodeDirectoryName_SavesSuccessfully()
        {
            var unicodeDir = Path.Combine( _testDirectory, "unicode目录" );
            Directory.CreateDirectory( unicodeDir );
            const string configContent = "test config content 1234 created on 6th january 2025";

            FileHandler.SaveConfigFile( unicodeDir, configContent );

            var filePath = Path.Combine( unicodeDir, Template.templateFileName );
            Assert.Multiple( () =>
            {
                Assert.That( File.Exists( filePath ), Is.True );
                Assert.That( File.ReadAllText( filePath ), Is.EqualTo( configContent ) );
            } );

            File.Delete( filePath ); //dont really need to do this since teardown deletes _testDirectory anyways but lets keep it here
        }
    }
}