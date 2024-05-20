namespace CNull.Common.Configuration
{
    /// <summary>
    /// Aggregates configuration parameters for the C? interpreter.
    /// </summary>
    public interface ICNullConfiguration
    {
        /// <summary>
        /// Maximum length of string literal.
        /// </summary>
        public int MaxStringLiteralLength { get; }

        /// <summary>
        /// Maximum length of a comment,
        /// </summary>
        public int MaxCommentLength { get; }

        /// <summary>
        /// Maximum length of an identifier.
        /// </summary>
        public int MaxIdentifierLength { get; }

        /// <summary>
        /// Maximum amount of subsequent whitespace characters.
        /// </summary>
        public int MaxWhitespaceLength { get; }

        /// <summary>
        /// Maximum token length.
        /// </summary>
        public int MaxTokenLength { get; }

        /// <summary>
        /// Maximum number of displayed compilation errors.
        /// </summary>
        public int MaxErrorsCount { get; }
    }
}
