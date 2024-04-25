using CNull.Lexer.Constants;

namespace CNull.Parser.Enums
{
    /// <summary>
    /// Enumeration of token types, which are primitive types specifiers.
    /// </summary>
    public enum PrimitiveTypes
    {
        Integer = TokenType.IntKeyword,
        Float = TokenType.FloatKeyword,
        String = TokenType.StringKeyword,
        Char = TokenType.CharKeyword,
        Boolean = TokenType.BoolKeyword,
    }

    /// <summary>
    /// Enumeration of token types, which are type specifiers.
    /// </summary>
    public enum Types
    {
        Integer = TokenType.IntKeyword,
        Float = TokenType.FloatKeyword,
        String = TokenType.StringKeyword,
        Char = TokenType.CharKeyword,
        Boolean = TokenType.BoolKeyword,
        Dictionary = TokenType.DictKeyword,
        Void = TokenType.VoidKeyword,
    }
}
