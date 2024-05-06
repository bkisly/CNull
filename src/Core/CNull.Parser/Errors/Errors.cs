using CNull.Common;
using CNull.ErrorHandler.Errors;

namespace CNull.Parser.Errors
{
    public class ExpectedIdentifierError(Position position) 
        : SyntaxError("Expected a valid identifier.", position);

    public class MissingKeywordOrOperatorError(string expectedText, Position position) 
        : SyntaxError($"Expected the following keyword or operator: {expectedText}", position);

    public class TypeNotPrimitiveError(Position position) 
        : SyntaxError("Usage of non-primitive type is not valid in this context.", position);

    public class ExpectedParameterError(Position position) 
        : SyntaxError("Expected a valid parameter.", position);

    public class ExpectedArgumentError(Position position)
        : SyntaxError("Expected a valid argument expression.", position);

    public class ExpectedStringLiteralError(Position position)
        : SyntaxError("Expected a valid string literal.", position);

    public class MissingCatchClauseError(Position position) 
        : SyntaxError("Expected at least 1 catch clause in try-catch statement.", position);

    public class InvalidExpressionError(Position position)
        : SyntaxError("Expected a valid expression.", position);

    public class InvalidLiteralError<T>(Position position)
        : SyntaxError($"Expected a literal of type: {typeof(T).Name}", position);

    public class ExpectedBlockStatementError(Position position)
        : SyntaxError("Expected a valid block statement.", position);
    public class InvalidReturnTypeError(Position position)
        : SyntaxError("Expected a valid return type.", position);
}
