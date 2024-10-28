using System.Data.SQLite;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace VR_Koni.ResultGraph
{
    /// <summary>
    /// <see cref="Decompressor{T}"/> is responsible for handling the extraction and conversion of 
    /// the compressed data stored in the SQLite file into a structured dataset like a list of tuples.
    /// Ensure that <EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization> is set in .csproj to use BinaryFormatter.
    /// </summary>
    public class Decompressor<T>
    {
        private const string DefaultQuery = @"SELECT depthData FROM surgery";
        private readonly int _duplicateCountTolerance;

        /// <summary>
        /// Parameterized constructor for <see cref="Decompressor{T}"/>
        /// </summary>
        /// <param name="duplicateCountTolerance"></param>
        public Decompressor( int duplicateCountTolerance = 3 )
        {
            _duplicateCountTolerance = duplicateCountTolerance;
        }

        /// <summary>
        /// Decompresses the x64 string from the specified SQLite file and returns it as 
        /// the specified type T. This method also provides an option to sanitize 
        /// the data by limiting duplicate consecutive values for which the threshold is 
        /// defined in <see cref="_duplicateCountTolerance"/>
        /// </summary>
        /// <param name="sqliteFileName">The path to the SQLite file.</param>
        /// <param name="query">The SQL query to execute for data retrieval.</param>
        /// <param name="sanitizeData">A flag indicating whether to sanitize the data, taking tolerance as <see cref="_duplicateCountTolerance"/>.</param>
        /// <returns>A list of type T representing the decompressed data.</returns>
        /// <exception cref="SQLiteException"></exception>
        /// <exception cref="Exception"></exception>
        public List<T>? DecompressDataFromSQLite(
            string sqliteFileName,
            string query = DefaultQuery,
            bool sanitizeData = true )
        {
            List<T>? cuttingData = new();
            try
            {
                string connectionString = $"Data Source={sqliteFileName};Version=3;";
                using SQLiteConnection connection = new( connectionString );
                connection.Open();

                using SQLiteCommand command = new( query, connection );
                using SQLiteDataReader reader = command.ExecuteReader();

                Sanitizer? sanitizer = sanitizeData ? new( _duplicateCountTolerance ) : null;
                while( reader.Read() )
                {
                    string compressedBase64 = reader.GetString( 0 );
                    byte[] compressedBytes = Convert.FromBase64String( compressedBase64 );

                    //this will implicitly always be List<Tuple<float,float>> in our case, just because of the way its stored in the DB
                    object decompressedData = DecompressData( compressedBytes );
                    List<T> processedData = ProcessDecompressedData( decompressedData );

                    cuttingData.AddRange( processedData );
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
        /// Serializes the provided data to JSON format 
        /// and writes it to the specified output file path. This allows for easy 
        /// storage and sharing of the decompressed data.
        /// </summary>
        /// <param name="outputFilePath">The path to the output JSON file.</param>
        /// <param name="data">The data to serialize and write.</param>
        public void WriteDataToJsonFile( List<T>? data, string outputFilePath = "output.json" )
        {
            //foreach( T t in data )
            //{
            //    Console.WriteLine( t?.ToString() );
            //}

            Container<T> container = new() { CuttingData = data };

            string jsonString = JsonConvert.SerializeObject( container, Formatting.Indented );
            //Console.WriteLine( jsonString );
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
        private object DecompressData( byte[] compressedBytes )
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
        /// Processes the decompressed data and converts it to the target type if necessary.
        /// </summary>
        private static List<T> ProcessDecompressedData( object data )
        {
            Type targetType = typeof( T );

            return data switch
            {
                List<Tuple<float, float>> tupleList when targetType == typeof( Vector2 ) =>
                    tupleList.Select( t => (T)(object)new Vector2( t.Item1, t.Item2 ) ).ToList(),

                List<Tuple<float, float>> tupleList when targetType == typeof( Tuple<float, float> ) =>
                    tupleList.Select( t => (T)(object)t ).ToList(),


                List<Vector2> vectorList when targetType == typeof( Tuple<float, float> ) =>
                    vectorList.Select( v => (T)(object)Tuple.Create( v.X, v.Y ) ).ToList(),

                List<Vector2> vectorList when targetType == typeof( Vector2 ) =>
                    vectorList.Select( v => (T)(object)v ).ToList(),

                _ => throw new InvalidCastException( $"Cannot convert from {data.GetType()} to List<{typeof( T )}>. " +
                        "Supported types as of now are Vector2 and Tuple<float, float>." )
            };
        }

        /// <summary>
        /// A simple container class for holding cutting data. This is used to 
        /// facilitate JSON serialization by encapsulating the list of tuples.
        /// </summary>
        public class Container<TT>
        {
            /// <summary>
            /// Container.
            /// </summary>
            public List<TT>? CuttingData { get; set; }
        }
    }

    public static class Program
    {
        /// <summary>
        /// Dummy entry point
        /// </summary>
        public static void Main()
        {
            string sqliteFilePath = @"C:\Users\kuike\Downloads\wetransfer_surgery-data_2024-10-25_1601\Surgery_Data\koni.sqlite";

            //Decompressor<Vector2> d = new();
            Decompressor<Tuple<float, float>> d = new();

            //List<Vector2>? cuttingData = d.DecompressDataFromSQLite( sqliteFilePath, sanitizeData: true );
            List<Tuple<float, float>>? cuttingData = d.DecompressDataFromSQLite( sqliteFilePath, sanitizeData: true );

            // DataPoints<Vector2> dp = new DataPoints<Vector2>( dummyPoints );
            // Console.WriteLine(dp.ToString());

            // Vector2 mt = dp.GetMinMaxTime();
            // Console.WriteLine($"{mt.x}, {mt.y}");

            // Vector2 md = dp.GetMinMaxDepth();
            // Console.WriteLine($"{mt.x}, {mt.y}");

            d.WriteDataToJsonFile( cuttingData, "dump1.json" );

        }
    }
}
