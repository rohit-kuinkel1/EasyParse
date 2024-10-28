using System.Data.SQLite;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace VR_Koni.ResultGraph
{
    /// <summary>
    /// <see cref="Decompressor"/> is responsible for handling the extraction and conversion of 
    /// the compressed data stored in the sqlite file into a structured dataset like a list of tuples.
    /// Ensure that <EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization> is set in .csproj to use BinaryFormatter.
    /// </summary>
    public static class Decompressor
    {
        private const string DefaultQuery = @"SELECT depthData FROM surgery";
        private static readonly int DuplicateCountTolerance = 3;

        /// <summary>
        /// dummy entry point
        /// </summary>
        public static void Main()
        {
            string sqliteFilePath = @"C:\Users\kuike\Downloads\wetransfer_surgery-data_2024-10-25_1601\Surgery_Data\koni.sqlite";

            List<Tuple<float, float>>? cuttingData = DecompressDataFromSQLite<List<Tuple<float, float>>>( sqliteFilePath, sanitizeData: true );

            WriteDataToJsonFile( "dump2.json", cuttingData );
        }

        /// <summary>
        /// Decompresses the x64 string from the specified SQLite file and returns it as a 
        /// specified type T. This method also provides an option to sanitize 
        /// the data by limiting duplicate consecutive values for which the threshold is 
        /// defined in <see cref="DuplicateCountTolerance"/>
        /// </summary>
        /// <typeparam name="T">The type of the data to return.</typeparam>
        /// <param name="sqliteFileName">The path to the SQLite file.</param>
        /// <param name="query">The SQL query to execute for data retrieval.</param>
        /// <param name="sanitizeData">A flag indicating whether to sanitize the data, taking tolerance as <see cref="DuplicateCountTolerance"/>.</param>
        /// <returns>An object of type T representing the decompressed data.</returns>
        /// <exception cref="SQLiteException"></exception>
        /// <exception cref="Exception"></exception>
        public static T? DecompressDataFromSQLite<T>(
            string sqliteFileName,
            string query = DefaultQuery,
            bool sanitizeData = true )
        {
            T? cuttingData = default;
            try
            {
                string connectionString = $"Data Source={sqliteFileName};Version=3;";
                using SQLiteConnection connection = new( connectionString );
                connection.Open();

                using SQLiteCommand command = new( query, connection );
                using SQLiteDataReader reader = command.ExecuteReader();

                Sanitizer? sanitizer = sanitizeData ? new( DuplicateCountTolerance ) : null;
                while( reader.Read() )
                {
                    string compressedBase64 = reader.GetString( 0 );
                    byte[] compressedBytes = Convert.FromBase64String( compressedBase64 );

                    cuttingData = (T)DecompressData( compressedBytes );

                    if( sanitizeData )
                    {
                        sanitizer?.SanitizeData( ref cuttingData );
                    }
                }
            }
            catch( SQLiteException ex )
            {
                Console.WriteLine( $"SQLite Error: {ex.Message}" );
                Console.WriteLine( ex.StackTrace );
            }
            catch( Exception ex )
            {
                Console.WriteLine( $"Unexpected Error: {ex.Message}" );
                Console.WriteLine( ex.StackTrace );
            }

            return cuttingData;
        }

        /// <summary>
        /// Serializes the provided List of Tuple<float, float> data to JSON format 
        /// and writes it to the specified output file path. This allows for easy 
        /// storage and sharing of the decompressed data.
        /// </summary>
        /// <param name="outputFilePath">The path to the output JSON file.</param>
        /// <param name="data">The List of Tuple<float, float> to serialize and write.</param>
        public static void WriteDataToJsonFile(
            string outputFilePath,
            List<Tuple<float, float>>? data )
        {
            Container container = new() { CuttingData = data };
            JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize( container, jsonOptions );
            File.WriteAllText( outputFilePath, jsonString );

            Console.WriteLine( $"Wrote to JSON File: {outputFilePath}..." );
        }

        /// <summary>
        /// Decompresses the provided byte array using GZip and deserializes it 
        /// into its original object form using BinaryFormatter. This method is 
        /// central to transforming the compressed data into a usable format.
        /// </summary>
        /// <param name="compressedBytes">The byte array containing compressed data.</param>
        /// <returns>The deserialized object from the compressed data.</returns>
        private static object DecompressData( byte[] compressedBytes )
        {
            using MemoryStream compressedStream = new MemoryStream( compressedBytes );
            using MemoryStream decompressedStream = new MemoryStream();
            using GZipStream gzipStream = new GZipStream( compressedStream, CompressionMode.Decompress );

            // Copy the decompressed data into a new stream.
            gzipStream.CopyTo( decompressedStream );
            decompressedStream.Position = 0;

#pragma warning disable SYSLIB0011
            BinaryFormatter formatter = new BinaryFormatter();
            object data = formatter.Deserialize( decompressedStream );
#pragma warning restore SYSLIB0011
            return data;
        }

        /// <summary>
        /// A simple container class for holding cutting data. This is used to 
        /// facilitate JSON serialization by encapsulating the list of tuples.
        /// </summary>
        public class Container
        {
            public object? CuttingData { get; set; }
        }
    }
}
