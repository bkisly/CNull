using CNull.Lexer.Constants;

namespace CNull.Lexer
{
    public record Token(TokenType TokenType)
    { 
        /// <summary>
        /// Creates a new token of type Unknown.
        /// </summary>
        /// <returns></returns>
        public static Token Unknown() => new(TokenType.Unknown);
    }

    public record Token<T>(T Value, TokenType TokenType) : Token(TokenType);
}
