using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class UnterminatedCharLiteral(Position position) : CompilationError(position)
    {
        public override string Message =>
            "Char literal can contain only one character and must be terminated with single quote (')";
    }
}
