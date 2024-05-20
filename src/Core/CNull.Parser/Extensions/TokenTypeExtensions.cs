using CNull.Lexer.Constants;

namespace CNull.Parser.Extensions
{
    internal static class TokenTypeExtensions
    {
        /// <summary>
        /// Determines whether given token type is a primitive type specifier.
        /// </summary>
        internal static bool IsPrimitiveType(this TokenType type) => Enum.IsDefined(typeof(PrimitiveTypes), (int)type);

        /// <summary>
        /// Determines whether given token type can be specified as the type of variable.
        /// </summary>
        internal static bool IsDeclarableType(this TokenType type) =>
            type == TokenType.DictKeyword || IsPrimitiveType(type);
    }
}
