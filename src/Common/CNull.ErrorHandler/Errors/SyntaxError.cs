using CNull.Common;

namespace CNull.ErrorHandler.Errors
{
    /// <summary>
    /// General type of a syntactic error.
    /// </summary>
    public abstract class SyntaxError(string message, Position position) : ICompilationError
    {
        public virtual string Message => $"Syntax error: {message}";
        public Position Position => position;
    }
}
