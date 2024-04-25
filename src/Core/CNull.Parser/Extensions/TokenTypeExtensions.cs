using CNull.Lexer.Constants;
using CNull.Parser.Enums;

namespace CNull.Parser.Extensions
{
    public static class TokenTypeExtensions
    {
        /// <summary>
        /// Determines whether given token type is a primitive type specifier.
        /// </summary>
        public static bool IsPrimitiveType(this TokenType type) => Enum.IsDefined(typeof(PrimitiveTypes), type);
    }
}
