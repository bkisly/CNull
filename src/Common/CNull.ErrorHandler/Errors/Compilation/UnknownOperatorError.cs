using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class UnknownOperatorError(Position position) : CompilationError(position)
    {
        public override string Message => "Unknown operator.";
    }
}
