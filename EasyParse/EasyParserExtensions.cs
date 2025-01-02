using EasyParser.Core;
namespace EasyParser
{
    /// <summary>
    /// Common extension class for <see cref="EasyParse"/>
    /// </summary>
    public static class EasyParseExtensions
    {
        /// <summary>
        /// Parses a type from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1>( this EasyParse parser, string[] args )
            where T1 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            store.AddResult( result1 );
            return store;
        }

        /// <summary>
        /// Parses two types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            return store;
        }

        /// <summary>
        /// Parses three types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            return store;
        }

        /// <summary>
        /// Parses four types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            return store;
        }

        /// <summary>
        /// Parses five types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            return store;
        }

        /// <summary>
        /// Parses six types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            return store;
        }

        /// <summary>
        /// Parses seven types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            return store;
        }

        /// <summary>
        /// Parses eight types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            return store;
        }

        /// <summary>
        /// Parses nine types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            return store;
        }

        /// <summary>
        /// Parses ten types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            return store;
        }

        /// <summary>
        /// Parses eleven types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            return store;
        }

        /// <summary>
        /// Parses twelve types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            return store;
        }

        /// <summary>
        /// Parses thirteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            return store;
        }

        /// <summary>
        /// Parses fourteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
            where T14 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            var result14 = parser.ParseOne<T14>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            store.AddResult( result14 );
            return store;
        }

        /// <summary>
        /// Parses fifteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
            where T14 : class, new()
            where T15 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            var result14 = parser.ParseOne<T14>( args );
            var result15 = parser.ParseOne<T15>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            store.AddResult( result14 );
            store.AddResult( result15 );
            return store;
        }

        /// <summary>
        /// Parses sixteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
            where T14 : class, new()
            where T15 : class, new()
            where T16 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            var result14 = parser.ParseOne<T14>( args );
            var result15 = parser.ParseOne<T15>( args );
            var result16 = parser.ParseOne<T16>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            store.AddResult( result14 );
            store.AddResult( result15 );
            store.AddResult( result16 );
            return store;
        }

        /// <summary>
        /// Parses seventeen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
            where T14 : class, new()
            where T15 : class, new()
            where T16 : class, new()
            where T17 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            var result14 = parser.ParseOne<T14>( args );
            var result15 = parser.ParseOne<T15>( args );
            var result16 = parser.ParseOne<T16>( args );
            var result17 = parser.ParseOne<T17>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            store.AddResult( result14 );
            store.AddResult( result15 );
            store.AddResult( result16 );
            store.AddResult( result17 );
            return store;
        }

        /// <summary>
        /// Parses eighteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
            where T14 : class, new()
            where T15 : class, new()
            where T16 : class, new()
            where T17 : class, new()
            where T18 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            var result14 = parser.ParseOne<T14>( args );
            var result15 = parser.ParseOne<T15>( args );
            var result16 = parser.ParseOne<T16>( args );
            var result17 = parser.ParseOne<T17>( args );
            var result18 = parser.ParseOne<T18>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            store.AddResult( result14 );
            store.AddResult( result15 );
            store.AddResult( result16 );
            store.AddResult( result17 );
            store.AddResult( result18 );
            return store;
        }

        /// <summary>
        /// Parses nineteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
            where T14 : class, new()
            where T15 : class, new()
            where T16 : class, new()
            where T17 : class, new()
            where T18 : class, new()
            where T19 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            var result14 = parser.ParseOne<T14>( args );
            var result15 = parser.ParseOne<T15>( args );
            var result16 = parser.ParseOne<T16>( args );
            var result17 = parser.ParseOne<T17>( args );
            var result18 = parser.ParseOne<T18>( args );
            var result19 = parser.ParseOne<T19>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            store.AddResult( result14 );
            store.AddResult( result15 );
            store.AddResult( result16 );
            store.AddResult( result17 );
            store.AddResult( result18 );
            store.AddResult( result19 );
            return store;
        }

        /// <summary>
        /// Parses twenty types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>( this EasyParse parser, string[] args )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
            where T8 : class, new()
            where T9 : class, new()
            where T10 : class, new()
            where T11 : class, new()
            where T12 : class, new()
            where T13 : class, new()
            where T14 : class, new()
            where T15 : class, new()
            where T16 : class, new()
            where T17 : class, new()
            where T18 : class, new()
            where T19 : class, new()
            where T20 : class, new()
        {
            var store = new ParsingResultStore();
            var result1 = parser.ParseOne<T1>( args );
            var result2 = parser.ParseOne<T2>( args );
            var result3 = parser.ParseOne<T3>( args );
            var result4 = parser.ParseOne<T4>( args );
            var result5 = parser.ParseOne<T5>( args );
            var result6 = parser.ParseOne<T6>( args );
            var result7 = parser.ParseOne<T7>( args );
            var result8 = parser.ParseOne<T8>( args );
            var result9 = parser.ParseOne<T9>( args );
            var result10 = parser.ParseOne<T10>( args );
            var result11 = parser.ParseOne<T11>( args );
            var result12 = parser.ParseOne<T12>( args );
            var result13 = parser.ParseOne<T13>( args );
            var result14 = parser.ParseOne<T14>( args );
            var result15 = parser.ParseOne<T15>( args );
            var result16 = parser.ParseOne<T16>( args );
            var result17 = parser.ParseOne<T17>( args );
            var result18 = parser.ParseOne<T18>( args );
            var result19 = parser.ParseOne<T19>( args );
            var result20 = parser.ParseOne<T20>( args );
            store.AddResult( result1 );
            store.AddResult( result2 );
            store.AddResult( result3 );
            store.AddResult( result4 );
            store.AddResult( result5 );
            store.AddResult( result6 );
            store.AddResult( result7 );
            store.AddResult( result8 );
            store.AddResult( result9 );
            store.AddResult( result10 );
            store.AddResult( result11 );
            store.AddResult( result12 );
            store.AddResult( result13 );
            store.AddResult( result14 );
            store.AddResult( result15 );
            store.AddResult( result16 );
            store.AddResult( result17 );
            store.AddResult( result18 );
            store.AddResult( result19 );
            store.AddResult( result20 );
            return store;
        }
    }
}