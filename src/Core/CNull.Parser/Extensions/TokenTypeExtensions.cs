using CNull.Lexer.Constants;
using CNull.Parser.Enums;

namespace CNull.Parser.Extensions
{
    public static class TokenTypeExtensions
    {
        /// <summary>
        /// Determines whether given token type is a primitive type specifier.
        /// </summary>
        public static bool IsPrimitiveType(this TokenType type) => Enum.IsDefined(typeof(PrimitiveTypes), (int)type);

        /// <summary>
        /// Determines whether given token type can be specified as a return type.
        /// </summary>
        public static bool IsReturnType(this TokenType type) => Enum.IsDefined(typeof(Types), (int)type);

        /// <summary>
        /// Determines whether given token type can be specified as the type of a variable.
        /// </summary>
        public static bool IsDeclarableType(this TokenType type) =>
            type == TokenType.DictKeyword || IsPrimitiveType(type);

        /// <summary>
        /// Determines whether given token type is a literal type.
        /// </summary>
        public static bool IsLiteral(this TokenType type)
        {
            var literals = new[]
            {
                TokenType.StringLiteral,
                TokenType.IntegerLiteral,
                TokenType.FloatLiteral,
                TokenType.CharLiteral,
                TokenType.TrueKeyword,
                TokenType.FalseKeyword,
                TokenType.NullKeyword
            };

            return literals.Contains(type);
        }
    }
}
