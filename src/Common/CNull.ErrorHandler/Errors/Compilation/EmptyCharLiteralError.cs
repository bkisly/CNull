using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class EmptyCharLiteralError(Position position) : CompilationError(position)
    {
        public override string Message => "Empty char literal.";
    }
}
