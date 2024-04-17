using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class NumericValueOverflowError(Position position) : CompilationError(position)
    {
        public override string Message => "Declared numeric value is outside accepted boundaries.";
    }
}
