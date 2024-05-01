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

        /// <summary>
        /// Determines whether given token type can be specified as a return type.
        /// </summary>
        public static bool IsReturnType(this TokenType type) => Enum.IsDefined(typeof(Types), type);

        /// <summary>
        /// Determines whether given token type can be specified as the type of a variable.
        /// </summary>
        public static bool IsDeclarableType(this TokenType type) =>
            type == TokenType.DictKeyword || IsPrimitiveType(type);
    }
}
