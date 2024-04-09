using CNull.Common;
using CNull.Lexer.Constants;

namespace CNull.Lexer
{
    /// <summary>
    /// Represents a single lexical unit.
    /// </summary>
    /// <param name="TokenType">The type of the token.</param>
    /// <param name="Position">The position of the token.</param>
    public record Token(TokenType TokenType, Position Position)
    { 
        /// <summary>
        /// Creates a new token of type Unknown with the given position.
        /// </summary>
        /// <returns></returns>
        public static Token Unknown(Position position) => new(TokenType.Unknown, position);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T">Type of the value of the token.</typeparam>
    /// <param name="Value">The value of the token.</param>
    /// <param name="TokenType"><inheritdoc/></param>
    /// <param name="Position"><inheritdoc/></param>
    public record Token<T>(T Value, TokenType TokenType, Position Position) : Token(TokenType, Position);
}
