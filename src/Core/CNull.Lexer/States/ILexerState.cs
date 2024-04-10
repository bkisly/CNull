namespace CNull.Lexer.States
{
    /// <summary>
    /// Encapsulates logic for currently built token.
    /// </summary>
    public interface ILexerState
    {
        /// <summary>
        /// Builds a token accordingly to the current state.
        /// </summary>
        /// <returns>The built token.</returns>
        Token BuildToken();
    }
}
