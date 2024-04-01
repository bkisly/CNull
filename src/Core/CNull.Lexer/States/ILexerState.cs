namespace CNull.Lexer.States
{
    /// <summary>
    /// Encapsulates logic for currently built token.
    /// </summary>
    public interface ILexerState
    {
        /// <summary>
        /// Tries to build a valid token.
        /// </summary>
        /// <param name="token">Token, which was built.</param>
        /// <returns><c>true</c> if building succeeded, <c>false</c> otherwise.</returns>
        bool TryBuildToken(out Token token);
    }
}
