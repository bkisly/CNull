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
        /// Number of the line at which the error occurred.
        /// </summary>
        int LineNumber { get; init; }

        /// <summary>
        /// Number of the column at which the error occurred.
        /// </summary>
        int ColumnNumber { get; init; }
    }

    /// <summary>
    /// Error (exception) which occurred at runtime.
    /// </summary>
    public interface IRuntimeError : IError
    {
        /// <summary>
        /// Number of the line at which the error occurred.
        /// </summary>
        int LineNumber { get; init; }
    }
}
