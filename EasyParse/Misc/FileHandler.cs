using System;
using System.IO;
using System.Reflection;

namespace EasyParse.Misc
{
    internal static class FileHandler
    {
        public static string? GetInitialDirectory()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if( entryAssembly == null )
            {
                Console.WriteLine( "Unable to determine the entry assembly." );
                return default;
            }

            var currentDirectory = Path.GetDirectoryName( entryAssembly.Location );
            if( currentDirectory == null )
            {
                Console.WriteLine( "Unable to determine the current directory." );
                return default;
            }

            return currentDirectory;
        }

        public static bool TestDirectoryWriteAccess( string directory )
        {
            try
            {
                var testFile = Path.Combine( directory, "test_write_permission.tmp" );
                File.WriteAllText( testFile, "test" );
                File.Delete( testFile );
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
                Console.WriteLine( $"Param {nameof( startDirectory )} was null when it was not supposed to be. Cannot export default config" );
                return default;
            }

            var currentDirectory = startDirectory;
            for( int i = 0; i < 3; i++ )
            {
                var parentDirectory = Directory.GetParent( currentDirectory )?.FullName;
                if( parentDirectory == null )
                {
                    Console.WriteLine( $"Reached the root directory. Using current directory: {currentDirectory}" );
                    break;
                }

                if( TestDirectoryWriteAccess( parentDirectory ) )
                {
                    currentDirectory = parentDirectory;
                }
                else
                {
                    Console.WriteLine( $"Cannot access or write to parent directory. Using current directory: {currentDirectory}" );
                    break;
                }
            }

            return currentDirectory;
        }

        /// <summary>
        /// Saves the config provided in <paramref name="configContent"/> to path <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="configContent"></param>
        public static void SaveConfigFile( string directory, string configContent )
        {
            string finalPath = string.Empty;
            try
            {
                finalPath = Directory.Exists( directory )
                   ? directory
                   : Directory.GetCurrentDirectory();

                var filePath = Path.Combine( finalPath, "EasyParseOptions.cs" );
                File.WriteAllText( filePath, configContent );
                Console.WriteLine( $"Configuration code has been saved to: {filePath}" );
            }
            catch( Exception ex )
            {
                Console.WriteLine( $"Unable to save file in {finalPath}. Error: {ex.Message}" );
            }
        }
    }
}
