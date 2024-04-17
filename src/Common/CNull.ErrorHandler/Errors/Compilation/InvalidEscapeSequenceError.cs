using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class InvalidEscapeSequenceError(Position position) : CompilationError(position)
    {
        public override string Message => "Cannot recognize given escape sequence.";
    }
}
