using CNull.Common;
using CNull.ErrorHandler.Errors;

namespace CNull.Lexer.Errors
{
    public record EmptyCharLiteralError(Position Position) 
        : LexicalError("Empty char literal.", Position);

    public record InvalidEscapeSequenceError(Position Position)
        : LexicalError("Cannot recognize given escape sequence.", Position);

    public record InvalidIdentifierError(Position Position) 
        : LexicalError("Identifiers can only contain letters, numbers and underscores, and cannot start with numbers.", Position);

    public record InvalidTokenLengthError(Position Position, int ExpectedLength) 
        : LexicalError($"Invalid length of analyzed literal/identifier. Maximum accepted length: {ExpectedLength}", Position);

    public record LineBreakedTextLiteralError(Position Position)
        : LexicalError("String or char literals cannot be line-breaked.", Position);

    public record NumericValueOverflowError(Position Position)
        : LexicalError("Declared numeric value is outside accepted boundaries.", Position);

    public record PrefixedZeroError(Position Position)
        : LexicalError("Numeric literal cannot have excessive zeros as a prefix.", Position);

    public record UnknownOperatorError(Position Position) 
        : LexicalError("Unknown operator.", Position);

    public record UnterminatedCharLiteralError(Position Position) 
        : LexicalError("Char literal can contain only one character and must be terminated with single quote (')", Position);
}
