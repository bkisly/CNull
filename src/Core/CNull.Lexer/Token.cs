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
    /// <inheritdoc/> This token can store a value.
    /// </summary>
    /// <typeparam name="T">Type of the value of the token.</typeparam>
    public record Token<T> : Token
    {
        /// <summary>
        /// The value of the token.
        /// </summary>
        public T Value { get; }

        public Token(T value, TokenType tokenType, Position position) : base(tokenType, position)
        {
            var validValueAndType = value switch
            {
                int => tokenType is TokenType.IntegerLiteral,
                string => tokenType is TokenType.StringLiteral or TokenType.Identifier or TokenType.Comment,
                float => tokenType is TokenType.FloatLiteral,
                char => tokenType is TokenType.CharLiteral,
                bool => tokenType is TokenType.TrueKeyword or TokenType.FalseKeyword,
                null => tokenType is TokenType.NullKeyword,
                _ => false
            };

            if (!validValueAndType)
                throw new ArgumentException(
                    $"When instantiating a token, composition of type: {value?.GetType().Name ?? "null"} and token type: {tokenType} is invalid.");

            Value = value;
        }
    }
}
