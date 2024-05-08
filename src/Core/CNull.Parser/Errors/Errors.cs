using CNull.Common;
using CNull.ErrorHandler.Errors;

namespace CNull.Parser.Errors
{
    public record ExpectedIdentifierError(Position Position) 
        : SyntaxError("Expected a valid identifier.", Position);

    public record MissingKeywordOrOperatorError(string ExpectedText, Position Position) 
        : SyntaxError($"Expected the following keyword or operator: {ExpectedText}", Position);

    public record TypeNotPrimitiveError(Position Position) 
        : SyntaxError("Usage of non-primitive type is not valid in this context.", Position);

    public record ExpectedParameterError(Position Position) 
        : SyntaxError("Expected a valid parameter.", Position);

    public record ExpectedArgumentError(Position Position)
        : SyntaxError("Expected a valid argument expression.", Position);

    public record ExpectedStringLiteralError(Position Position)
        : SyntaxError("Expected a valid string literal.", Position);

    public record MissingCatchClauseError(Position Position) 
        : SyntaxError("Expected at least 1 catch clause in try-catch statement.", Position);

    public record InvalidExpressionError(Position Position)
        : SyntaxError("Expected a valid expression.", Position);

    public record InvalidLiteralError<T>(Position Position)
        : SyntaxError($"Expected a literal of type: {typeof(T).Name}", Position);

    public record ExpectedBlockStatementError(Position Position)
        : SyntaxError("Expected a valid block statement.", Position);
    public record InvalidReturnTypeError(Position Position)
        : SyntaxError("Expected a valid return type.", Position);
}
