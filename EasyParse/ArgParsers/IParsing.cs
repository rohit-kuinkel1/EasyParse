using System;

namespace EasyParser.Parsing
{
    /// <summary>
    /// contract for all the parsers to follow
    /// </summary>
    public interface IParsing
    {
        /// <summary>
        /// Parses the provided <paramref name="args"/> to be used and stored internally for later use.
        /// <paramref name="type"/> represents the type of class to be reflected using reflection.
        /// Passing type will significantly reduce the reflection overhead since we won't need to reflect many classes.
        /// If <paramref name="type"/> is omitted, the reflection cost will be significantly greater which will impact runtime performance.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="type"></param>
        bool Parse( string[] args, Type? type );
    }
}
