using CNull.Common;

namespace CNull.ErrorHandler.Errors
{
    /// <summary>
    /// General type of lexical error.
    /// </summary>
    public record LexicalError(string InnerMessage, Position Position) : ICompilationError
    {
        public string Message => $"Lexical error: {InnerMessage}";
    }
}
