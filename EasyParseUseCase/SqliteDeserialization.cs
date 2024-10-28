﻿using System.Data.SQLite;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

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
        private static readonly int DuplicateCountTolerance = 3;

        /// <summary>
        /// Decompresses the x64 string from the specified SQLite file and returns it as 
        /// the specified type T. This method also provides an option to sanitize 
        /// the data by limiting duplicate consecutive values for which the threshold is 
        /// defined in <see cref="DuplicateCountTolerance"/>
        /// </summary>
        /// <param name="sqliteFileName">The path to the SQLite file.</param>
        /// <param name="query">The SQL query to execute for data retrieval.</param>
        /// <param name="sanitizeData">A flag indicating whether to sanitize the data, taking tolerance as <see cref="DuplicateCountTolerance"/>.</param>
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

                Sanitizer? sanitizer = sanitizeData ? new( DuplicateCountTolerance ) : null;
                while( reader.Read() )
                {
                    string compressedBase64 = reader.GetString( 0 );
                    byte[] compressedBytes = Convert.FromBase64String( compressedBase64 );

                    //this will implicitly always be Tuple<float,float> in our case, just because of the way its stored in the DB
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
            Container<T> container = new() { CuttingData = data };
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

            if( data is List<Tuple<float, float>> tupleList )
            {
                if( targetType == typeof( Vector2 ) )
                {
                    return tupleList.Select( t => (T)(object)new Vector2( t.Item1, t.Item2 ) ).ToList();
                }
                else if( targetType == typeof( Tuple<float, float> ) )
                {
                    return tupleList.Select( t => (T)(object)t ).ToList();
                }
            }
            else if( data is List<Vector2> vectorList )
            {
                if( targetType == typeof( Tuple<float, float> ) )
                {
                    return vectorList.Select( v => (T)(object)Tuple.Create( v.X, v.Y ) ).ToList();
                }
                else if( targetType == typeof( Vector2 ) )
                {
                    return vectorList.Select( v => (T)(object)v ).ToList();
                }
            }

            throw new InvalidCastException(
                $"Cannot convert from {data.GetType()} to List<{typeof( T )}>. " +
                "Supported types are Vector2 and Tuple<float, float>." );
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

            Decompressor<Vector2> d = new();

            //Decompressor<Tuple<float, float>> d = new();

            string sqliteFilePath = @"C:\Users\kuike\Downloads\wetransfer_surgery-data_2024-10-25_1601\Surgery_Data\koni.sqlite";

            List<Vector2>? cuttingData = d.DecompressDataFromSQLite( sqliteFilePath, sanitizeData: true );
           // List<Tuple<float, float>>? cuttingData = d.DecompressDataFromSQLite( sqliteFilePath, sanitizeData: true );
            d.WriteDataToJsonFile( cuttingData, "dump1.json" );

        }
    }
}