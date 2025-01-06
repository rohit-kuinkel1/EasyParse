using EasyParser.Core;
using EasyParser.Misc;

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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses two types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2>(
            this EasyParse parser,
            string[] args
        )
             where T1 : class, new()
             where T2 : class, new()
        {
            var store = new ParsingResultStore();
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses three types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3>(
            this EasyParse parser,
            string[] args
        )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            var store = new ParsingResultStore();
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses four types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4>(
            this EasyParse parser,
            string[] args
        )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            var store = new ParsingResultStore();
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses five types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5>(
            this EasyParse parser,
            string[] args
        )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
        {
            var store = new ParsingResultStore();
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses six types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6>(
            this EasyParse parser,
            string[] args
        )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
        {
            var store = new ParsingResultStore();
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result6 = parser.ParseOne<T6>( group );
                    store.AddResult( result6 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses seven types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7>(
            this EasyParse parser,
            string[] args
        )
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new()
            where T6 : class, new()
            where T7 : class, new()
        {
            var store = new ParsingResultStore();
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result6 = parser.ParseOne<T6>( group );
                    store.AddResult( result6 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result7 = parser.ParseOne<T7>( group );
                    store.AddResult( result7 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses eight types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8>(
            this EasyParse parser,
            string[] args
        )
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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result6 = parser.ParseOne<T6>( group );
                    store.AddResult( result6 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result7 = parser.ParseOne<T7>( group );
                    store.AddResult( result7 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T8>( verbName ) )
                {
                    var result8 = parser.ParseOne<T8>( group );
                    store.AddResult( result8 );
                }
            }

            return store;
        }

        /// <summary>
        /// Parses nine types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this EasyParse parser,
            string[] args
        )
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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result6 = parser.ParseOne<T6>( group );
                    store.AddResult( result6 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result7 = parser.ParseOne<T7>( group );
                    store.AddResult( result7 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T8>( verbName ) )
                {
                    var result8 = parser.ParseOne<T8>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T9>( verbName ) )
                {
                    var result8 = parser.ParseOne<T9>( group );
                    store.AddResult( result8 );
                }
            }
            return store;
        }

        /// <summary>
        /// Parses ten types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this EasyParse parser,
            string[] args
        )
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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result6 = parser.ParseOne<T6>( group );
                    store.AddResult( result6 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result7 = parser.ParseOne<T7>( group );
                    store.AddResult( result7 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T8>( verbName ) )
                {
                    var result8 = parser.ParseOne<T8>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T9>( verbName ) )
                {
                    var result8 = parser.ParseOne<T9>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T10>( verbName ) )
                {
                    var result8 = parser.ParseOne<T10>( group );
                    store.AddResult( result8 );
                }
            }
            return store;
        }

        /// <summary>
        /// Parses eleven types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this EasyParse parser,
            string[] args
        )
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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result6 = parser.ParseOne<T6>( group );
                    store.AddResult( result6 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result7 = parser.ParseOne<T7>( group );
                    store.AddResult( result7 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T8>( verbName ) )
                {
                    var result8 = parser.ParseOne<T8>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T9>( verbName ) )
                {
                    var result8 = parser.ParseOne<T9>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T10>( verbName ) )
                {
                    var result8 = parser.ParseOne<T10>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T11>( verbName ) )
                {
                    var result8 = parser.ParseOne<T11>( group );
                    store.AddResult( result8 );
                }
            }
            return store;
        }

        /// <summary>
        /// Parses twelve types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this EasyParse parser,
            string[] args
        )
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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result1 = parser.ParseOne<T1>( group );
                    store.AddResult( result1 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result2 = parser.ParseOne<T2>( group );
                    store.AddResult( result2 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result3 = parser.ParseOne<T3>( group );
                    store.AddResult( result3 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result4 = parser.ParseOne<T4>( group );
                    store.AddResult( result4 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result5 = parser.ParseOne<T5>( group );
                    store.AddResult( result5 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result6 = parser.ParseOne<T6>( group );
                    store.AddResult( result6 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result7 = parser.ParseOne<T7>( group );
                    store.AddResult( result7 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T8>( verbName ) )
                {
                    var result8 = parser.ParseOne<T8>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T9>( verbName ) )
                {
                    var result8 = parser.ParseOne<T9>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T10>( verbName ) )
                {
                    var result8 = parser.ParseOne<T10>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T11>( verbName ) )
                {
                    var result8 = parser.ParseOne<T11>( group );
                    store.AddResult( result8 );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T12>( verbName ) )
                {
                    var result8 = parser.ParseOne<T12>( group );
                    store.AddResult( result8 );
                }
            }
            return store;
        }

        /// <summary>
        /// Parses thirteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this EasyParse parser,
            string[] args
        )
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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                //for a given string[], extract the VerbName from it, nothing fancy, just takes args[0]
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result = parser.ParseOne<T1>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result = parser.ParseOne<T2>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result = parser.ParseOne<T3>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result = parser.ParseOne<T4>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result = parser.ParseOne<T5>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result = parser.ParseOne<T6>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result = parser.ParseOne<T7>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T8>( verbName ) )
                {
                    var result = parser.ParseOne<T8>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T9>( verbName ) )
                {
                    var result = parser.ParseOne<T9>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T10>( verbName ) )
                {
                    var result = parser.ParseOne<T10>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T11>( verbName ) )
                {
                    var result = parser.ParseOne<T11>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T12>( verbName ) )
                {
                    var result = parser.ParseOne<T12>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T13>( verbName ) )
                {
                    var result = parser.ParseOne<T13>( group );
                    store.AddResult( result );
                }
            }
            return store;
        }

        /// <summary>
        /// Parses fourteen types simultaneously from the provided arguments
        /// </summary>
        public static ParsingResultStore Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this EasyParse parser,
            string[] args
        )
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
            var verbGroups = VerbParsingUtility.SplitVerbGroups( args );

            foreach( var group in verbGroups )
            {
                var verbName = VerbParsingUtility.GetVerbName( group );

                if( VerbParsingUtility.IsMatchingVerbType<T1>( verbName ) )
                {
                    var result = parser.ParseOne<T1>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T2>( verbName ) )
                {
                    var result = parser.ParseOne<T2>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T3>( verbName ) )
                {
                    var result = parser.ParseOne<T3>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T4>( verbName ) )
                {
                    var result = parser.ParseOne<T4>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T5>( verbName ) )
                {
                    var result = parser.ParseOne<T5>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T6>( verbName ) )
                {
                    var result = parser.ParseOne<T6>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T7>( verbName ) )
                {
                    var result = parser.ParseOne<T7>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T8>( verbName ) )
                {
                    var result = parser.ParseOne<T8>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T9>( verbName ) )
                {
                    var result = parser.ParseOne<T9>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T10>( verbName ) )
                {
                    var result = parser.ParseOne<T10>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T11>( verbName ) )
                {
                    var result = parser.ParseOne<T11>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T12>( verbName ) )
                {
                    var result = parser.ParseOne<T12>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T13>( verbName ) )
                {
                    var result = parser.ParseOne<T13>( group );
                    store.AddResult( result );
                }
                else if( VerbParsingUtility.IsMatchingVerbType<T14>( verbName ) )
                {
                    var result = parser.ParseOne<T14>( group );
                    store.AddResult( result );
                }
            }
            return store;
        }
    }
}