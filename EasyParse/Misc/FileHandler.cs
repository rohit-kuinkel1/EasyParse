using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using EasyParse;
namespace EasyParse.Misc
{
    [SupportedOSPlatform( "windows" )]
    [SupportedOSPlatform( "linux" )]
    internal static class FileHandler
    {
        public static string? GetInitialDirectory()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if( entryAssembly == null )
            {
                Logger.Error( "Unable to determine the entry assembly." );
                return default;
            }

            var currentDirectory = Path.GetDirectoryName( entryAssembly.Location );
            if( currentDirectory == null )
            {
                Logger.Error( "Unable to determine the current directory." );
                return default;
            }

            return currentDirectory;
        }

        public static bool TestDirectoryWriteAccess( string directory )
        {
            try
            {
                var testFile = Path.Combine( directory, "test_write_permission.tmp" );
                File.WriteAllText( path: testFile, contents: "test" );
                File.Delete( path: testFile );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string? FindWritableParentDirectory( string? startDirectory )
        {
            if( startDirectory == null )
            {
                Logger.Error( $"Param {nameof( startDirectory )} was null when it was not supposed to be. Cannot export default config" );
                return default;
            }

            var currentDirectory = startDirectory;
            for( int i = 0; i < 3; i++ )
            {
                var parentDirectory = Directory.GetParent( currentDirectory )?.FullName;
                if( parentDirectory == null )
                {
                    Logger.Info( $"Reached the root directory. Using current directory: {currentDirectory}" );
                    break;
                }

                if( TestDirectoryWriteAccess( parentDirectory ) )
                {
                    currentDirectory = parentDirectory;
                }
                else
                {
                    Logger.Warn( $"Cannot access or write to the parent directory of the CWD. Using current directory: {currentDirectory}" );
                    break;
                }
            }

            return currentDirectory;
        }

        /// <summary>
        /// Saves the config provided in <paramref name="configContent"/> to path <paramref name="directory"/>.
        /// It creates a .CS file with the name of <see cref="Template.templateFileName"/>
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="configContent"></param>
        public static void SaveConfigFile( string? directory, string configContent )
        {
            string finalPath = string.Empty;
            try
            {
                finalPath =
                    /*!string.IsNullOrEmpty( directory ) &&*/ Directory.Exists( directory )
                    ? directory
                    : Directory.GetCurrentDirectory();

                var filePath = Path.Combine( finalPath, Template.templateFileName );
                File.WriteAllText( filePath, configContent );
                Logger.Info( $"Configuration code has been saved to: {filePath}" );
            }
            catch( Exception ex )
            {
                Logger.Error( $"Unable to save file in {finalPath}. Error: {ex.Message}" );
            }
        }
    }
}
