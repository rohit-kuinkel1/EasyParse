namespace EasyParser.Enums
{
    /// <summary>
    /// Specifies the type of mutual relationship between options.
    /// </summary>
    public enum MutualType
    {
        /// <summary>
        /// defines a mutually exclusive relationship between 2 entities..
        /// </summary>
        Exclusive,

        /// <summary>
        /// defines a mutually inclusive relationship between 2 entities.
        /// </summary>
        Inclusive 
    }

    /// <summary>
    /// Keywords reserved for EasyParser in the args
    /// </summary>
    public enum ParsingKeyword
    {
        /// <summary>
        /// Denotes the start of the options
        /// </summary>
        Where,

        /// <summary>
        /// Denotes the value for the options, equivalent to =
        /// </summary>
        Is,

        /// <summary>
        /// Chains the options to a verb
        /// </summary>
        And
    }
}
