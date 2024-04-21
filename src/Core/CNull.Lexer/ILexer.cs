namespace CNull.Lexer
{
    /// <summary>
    /// Produces lexical tokens from consumed characters.
    /// </summary>
    public interface ILexer
    {
        /// <summary>
        /// Last built token.
        /// </summary>
        Token? LastToken { get; }

        /// <summary>
        /// Produces a token out of characters read from the source stream.
        /// </summary>
        /// <returns></returns>
        Token GetNextToken();
    }

    /// <summary>
    /// Marker interface, which indicates a lexer that returns raw, unfiltered or unprocessed tokens.
    /// </summary>
    public interface IRawLexer : ILexer;
}
