using CNull.Lexer.Constants;

namespace CNull.Lexer
{
    public record Token(TokenType TokenType);
    public record Token<T>(T Value, TokenType TokenType) : Token(TokenType);
}
