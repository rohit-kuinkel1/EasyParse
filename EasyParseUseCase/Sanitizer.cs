using System.Numerics;

namespace VR_Koni.ResultGraph
{
    /// <summary>
    /// The <see cref="Sanitizer"/> class provides methods to sanitize data collections
    /// by removing excessive consecutive duplicates. It is designed to handle 
    /// specific data types such as lists of tuples and vectors.
    /// </summary>
    public class Sanitizer
    {
        private readonly int _duplicateCountTolerance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sanitizer"/> class with a specified
        /// tolerance for consecutive duplicates.
        /// </summary>
        /// <param name="duplicateTolerance">The maximum number of consecutive duplicates allowed.</param>
        public Sanitizer( int duplicateTolerance )
        {
            _duplicateCountTolerance = duplicateTolerance;
        }

        /// <summary>
        /// Main sanitization method that routes the sanitization process to 
        /// type-specific implementations based on the provided data collection type.
        /// </summary>
        /// <typeparam name="T">The type of collection to sanitize.</typeparam>
        /// <param name="data">The data collection to sanitize, passed by reference.</param>
        public void SanitizeData<T>( ref T data )
        {
            switch( data )
            {
                case List<Tuple<float, float>> tupleList:
                    SanitizeTupleData( tupleList );
                    break;
                case List<Vector2> vectorList:
                    SanitizeVectorData( vectorList );
                    break;
                default:
                    throw new ArgumentException( $"Unsupported type for sanitization: {typeof( T )}" );
            }
        }

        /// <summary>
        /// Sanitizes a list of <see cref="Tuple{float, float}"/> data by removing
        /// excessive consecutive duplicates based on the Y value (Item2).
        /// </summary>
        /// <param name="data">The list of tuples to sanitize.</param>
        private void SanitizeTupleData( List<Tuple<float, float>> data )
        {
            if( data.Count < 2 )
            {
                return; // No sanitization needed for less than 2 items.
            }

            int duplicateCount = 1;
            for( int i = 1; i < data.Count; i++ )
            {
                if( data[i].Item2 == data[i - 1].Item2 )
                {
                    duplicateCount++;
                    if( duplicateCount > _duplicateCountTolerance )
                    {
                        data.RemoveAt( i );
                        i--;
                    }
                }
                else
                {
                    duplicateCount = 1; // Reset count for a new value of Item2.
                }
            }
        }

        /// <summary>
        /// Sanitizes a list of <see cref="Vector2"/> data by removing excessive 
        /// consecutive duplicates based on the Y value.
        /// </summary>
        /// <param name="data">The list of vectors to sanitize.</param>
        private void SanitizeVectorData( List<Vector2> data )
        {
            if( data.Count < 2 )
            {
                return; // No sanitization needed for less than 2 items.
            }

            int duplicateCount = 1;
            for( int i = 1; i < data.Count; i++ )
            {
                if( data[i].Y == data[i - 1].Y )
                {
                    duplicateCount++;
                    if( duplicateCount > _duplicateCountTolerance )
                    {
                        data.RemoveAt( i );
                        i--;
                    }
                }
                else
                {
                    duplicateCount = 1; // Reset count for a new value of Y.
                }
            }
        }
    }
}
