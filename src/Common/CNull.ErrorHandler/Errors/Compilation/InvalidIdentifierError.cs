using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class InvalidIdentifierError(Position position) : CompilationError(position)
    {
        public override string Message =>
            "Identifiers can only contain letters, numbers and underscores, and cannot start with numbers.";
    }
}
