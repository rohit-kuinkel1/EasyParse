using System;

namespace EasyParser.Utility
{
    /// <summary>
    /// Base class for all utility-related exceptions.
    /// https://github.com/rohit-kuinkel1/EverydayAutomation/blob/main/C%23/Exceptions.cs
    /// </summary>
    internal class EasyParseException : Exception
    {
        /// <summary>
        /// Denotes the prefix for each error message for easier source recognition.
        /// </summary>
        public static readonly string Prefix = "EasyParse ";

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyParseException"/> class.
        /// </summary>
        public EasyParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyParseException"/> class with a specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EasyParseException( string message )
            : base( $"{nameof( EasyParseException )} {message}" )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyParseException"/> class with a specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public EasyParseException(
            string message,
            Exception innerException )
            : base( message, innerException )
        {
        }
    }

    /// <summary>
    /// Exception thrown when a null argument is passed to a method that requires a non-null argument.
    /// </summary>
    internal class NullException : EasyParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullException"/> class with a specified parameter name.
        /// </summary>
        /// <param name="message">The error message.</param>
        public NullException( string message )
            : base( $"{nameof( EasyParseException )} {message}" )
        {
        }

        /// <summary>
        /// Default constructor for <see cref="NullException"/>
        /// </summary>
        public NullException()
            : base( $"{Prefix} Argument cannot be null." )
        {
        }

        /// <summary>
        /// Detailed parameterized constructor with <paramref name="type"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public NullException(
            string message,
            Type type )
            : base( $"{Prefix} {nameof(NullException)}, Type:{type} {message} " )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullException"/> class with a specified message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public NullException(
            string message,
            Exception innerException
            )
            : base( $"{Prefix} {nameof(NullException)}, {message}", innerException )
        {
        }
    }

    /// <summary>
    /// Exception thrown when an invalid value is encountered.
    /// </summary>
    internal class InvalidValueException : EasyParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException"/> class.
        /// </summary>
        public InvalidValueException()
            : base( $"{Prefix} {nameof(InvalidValueException)}, The provided value is invalid." )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException"/> class with a specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidValueException( string message )
            : base( $"{Prefix} {nameof( InvalidValueException )} {message}" )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException"/> class with a specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidValueException(
            string message,
            Exception innerException
            )
            : base( $"{Prefix} {nameof(InvalidValueException)}, {message}", innerException )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException"/> class with a specified message, inner exception, and invalid value object.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <param name="invalidValue">The invalid value that caused the exception.</param>
        public InvalidValueException(
            string message,
            Exception innerException,
            object invalidValue
            )
            : base( $"{Prefix} {nameof(InvalidValueException)}, {message} Invalid value type: {invalidValue.GetType().Name}", innerException )
        {
        }
    }


    /// <summary>
    /// Exception thrown when a fatal operation cannot continue.
    /// </summary>
    internal class IllegalOperation : EasyParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalOperation"/> class.
        /// </summary>
        public IllegalOperation()
            : base( $"{Prefix} {nameof(IllegalOperation)}, Cannot continue because of a fatal operation." )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalOperation"/> class with a specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IllegalOperation( string message )
            : base( $"{Prefix} {nameof( IllegalOperation )}, {message}" )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalOperation"/> class with a specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public IllegalOperation(
            string message,
            Exception innerException
            )
            : base( $"{Prefix} {nameof( IllegalOperation )}, {message}", innerException )
        {
        }
    }


    /// <summary>
    /// Exception thrown when a badly formatted argument is passed
    /// </summary>
    internal class BadFormatException : EasyParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadFormatException"/> class.
        /// </summary>
        public BadFormatException()
            : base( $"{Prefix} {nameof( BadFormatException )}, Argument did not match the expected structure." )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadFormatException"/> class with a specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BadFormatException( string message )
            : base( $"{Prefix} {nameof( BadFormatException )}, {message}" )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadFormatException"/> class with a specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BadFormatException(
            string message,
            Exception innerException
            )
            : base( $"{Prefix} {nameof( BadFormatException )}, {message}", innerException )
        {
        }
    }

    /// <summary>
    /// Exception thrown when deserialization failed.
    /// </summary>
    internal class DeserializationException : EasyParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationException"/> class.
        /// </summary>
        public DeserializationException()
            : base( $"{Prefix} {nameof( DeserializationException )}, Failed to deserialize an object." )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationException"/> class with a specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeserializationException( string message )
            : base( $"{Prefix} {nameof( DeserializationException )}, {message}" )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationException"/> class with a specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DeserializationException(
            string message,
            Exception innerException
            )
            : base( $"{Prefix} {nameof(DeserializationException)}, {message}", innerException )
        {
        }
    }

    /// <summary>
    /// Exception thrown when a file format that is not supported is used.
    /// </summary>
    internal class InvalidFileFormat : EasyParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFileFormat"/> class.
        /// </summary>
        public InvalidFileFormat()
            : base( $"{Prefix} {nameof(InvalidFileFormat)}, Cannot continue because of an invalid file format." )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFileFormat"/> class with a specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidFileFormat( string message )
            : base( $"{Prefix} {nameof( InvalidFileFormat )} {message}" )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFileFormat"/> class with a specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidFileFormat(
            string message,
            Exception innerException
            )
            : base( $"{Prefix} {nameof(InvalidFileFormat)}, {message}", innerException )
        {
        }
    }
}