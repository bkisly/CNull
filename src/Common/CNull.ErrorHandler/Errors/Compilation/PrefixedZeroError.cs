using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class PrefixedZeroError(Position position) : CompilationError(position)
    {
        public override string Message => "Numeric literal cannot have excessive zeros as a prefix.";
    }
}
