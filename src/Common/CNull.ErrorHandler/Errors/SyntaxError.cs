using CNull.Common;

namespace CNull.ErrorHandler.Errors
{
    /// <summary>
    /// General type of a syntactic error.
    /// </summary>
    public abstract record SyntaxError(string InnerMessage, Position Position) : ICompilationError
    {
        public virtual string Message => $"Syntax error: {InnerMessage}";
    }
}
