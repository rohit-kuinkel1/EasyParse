using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EasyParse.Utility
{
    /// <summary>
    /// Contains misc Utilities for use by the tool.
    /// https://github.com/rohit-kuinkel1/EverydayAutomation/blob/main/C%23/NotNullValidation.cs
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// Provides detailed context about the caller and checks if the specified object is null or empty.
        /// Throws a <see cref="NullException"/> if the object is null or empty and <paramref name="throwIfNull"/> is set to True.
        /// Else conducts a soft fail by returning false.
        /// If the object was not null or empty, then returns true.
        /// </summary>
        /// <typeparam name="T">The type of the object being checked.</typeparam>
        /// <param name="obj">The object to check for null or emptiness.</param>
        /// <param name="throwIfNull">Whether to throw an exception or just return false.</param>
        /// <param name="customErrorMessage">Optional custom error message.</param>
        /// <param name="parameterName">The name of the parameter being checked. This is optional and is automatically populated by the compiler.</param>
        /// <param name="memberName">The name of the method or property that called this check. This is automatically populated by the compiler.</param>
        /// <param name="filePath">The file path where the caller is located. This is automatically populated by the compiler.</param>
        /// <param name="lineNumber">The line number in the file where this check is called. This is automatically populated by the compiler.</param>
        /// <returns>True if the object is not null or empty, false otherwise (in soft fail mode).</returns>
        /// <exception cref="NullException">Thrown when the object is null or empty in hard fail mode.</exception>
        public static bool NotNullValidation<T>(
            [NotNullWhen( true )] T? obj,
            bool throwIfNull = true,
            string? customErrorMessage = null,
            [CallerArgumentExpression( "obj" )] string? parameterName = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0
        )
        {
            if( typeof( T ).IsValueType && !typeof( T ).IsNullable() )
            {
                return true; // Non-nullable value types like int, float, double, char, bool are never null
            }

            var isNullOrEmpty = obj switch
            {
                null => true,
                string s => string.IsNullOrWhiteSpace( s ),
                Array array => array.Length == 0,
                ICollection collection => collection.Count == 0,
                IEnumerable enumerable => !enumerable.GetEnumerator().MoveNext(),
                _ => false
            };

            if( isNullOrEmpty )
            {
                if( throwIfNull )
                {
                    var errorMessage = customErrorMessage ??
                        $"Parameter '{parameterName}' cannot be null or empty. " +
                        $"Called from {memberName} at {filePath} @line:{lineNumber}";
                    throw new NullException( errorMessage, typeof( T ) );
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the underlying type of the given <see cref="Type"/> is not null.
        /// Returns with true if it was not null else returns with a false.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable( this Type type )
        {
            return Nullable.GetUnderlyingType( type ) != null;
        }
    }
}