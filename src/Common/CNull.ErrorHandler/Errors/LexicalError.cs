using CNull.Common;

namespace CNull.ErrorHandler.Errors
{
    /// <summary>
    /// General type of a lexical error.
    /// </summary>
    public class LexicalError(string message, Position position) : ICompilationError
    {
        public string Message => $"Lexical error: {message}";
        public Position Position => position;
    }
}
