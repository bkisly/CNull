using CNull.Lexer.Constants;

namespace CNull.Lexer
{
    /// <summary>
    /// Represents a single lexical unit.
    /// </summary>
    /// <param name="TokenType">The type of the token.</param>
    public record Token(TokenType TokenType)
    { 
        /// <summary>
        /// Creates a new token of type Unknown.
        /// </summary>
        /// <returns></returns>
        public static Token Unknown() => new(TokenType.Unknown);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T">Type of the value of the token.</typeparam>
    /// <param name="Value">The value of the token.</param>
    /// <param name="TokenType"><inheritdoc/></param>
    public record Token<T>(T Value, TokenType TokenType) : Token(TokenType);
}
