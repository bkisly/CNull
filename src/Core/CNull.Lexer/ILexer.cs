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
}
