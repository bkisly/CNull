using CNull.Common;

namespace CNull.ErrorHandler.Errors
{
    /// <summary>
    /// Base error interface.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Descriptive information about the error.
        /// </summary>
        string Message { get; }
    }

    /// <summary>
    /// Error which occurred during interacting with the source of the code.
    /// </summary>
    public interface ISourceError : IError;

    /// <summary>
    /// Error which occurred during static analysis of the code.
    /// </summary>
    public interface ICompilationError : IError
    {
        /// <summary>
        /// Position at which the error occurred.
        /// </summary>
        Position Position { get; }
    }

    /// <summary>
    /// Error which occurred during semantic analysis of the code.
    /// </summary>
    public interface ISemanticError : IError
    {
        /// <summary>
        /// Name of the module in which the error occurred.
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// Number of the line at which error occurred.
        /// </summary>
        int? LineNumber { get; }
    }

    /// <summary>
    /// Error which occurred at runtime.
    /// </summary>
    public interface IRuntimeError : IError
    {
       IEnumerable<CallStackRecord> CallStack { get; }
    }
}
