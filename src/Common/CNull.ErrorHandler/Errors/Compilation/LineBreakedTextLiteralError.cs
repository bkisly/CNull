using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class LineBreakedTextLiteralError(Position position) : CompilationError(position)
    {
        public override string Message => "String or char literal cannot be line-breaked.";
    }
}
