using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public abstract class CompilationError(Position position) : ICompilationError
    {
        public abstract string Message { get; }
        public Position Position => position;
    }
}
