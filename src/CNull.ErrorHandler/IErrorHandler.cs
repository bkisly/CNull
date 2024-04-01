using CNull.ErrorHandler.Errors;

namespace CNull.ErrorHandler
{
    /// <summary>
    /// Provides an interface to raise errors and effectively redirect them to the user.
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Event raised when an error occurred.
        /// </summary>
        event EventHandler<ErrorEventArgs> ErrorOccurred;

        /// <summary>
        /// Raises an error which occurred during interacting with the source.
        /// </summary>
        /// <param name="error">Error to raise.</param>
        void RaiseSourceError(ISourceError error);


        /// <summary>
        /// Raises an error which occurred during static analysis of the code.
        /// </summary>
        /// <param name="error">Error to raise.</param>
        void RaiseCompilationError(ICompilationError error);

        /// <summary>
        /// Raises an error which occurred at runtime.
        /// </summary>
        /// <param name="error">Error to raise.</param>
        void RaiseRuntimeError(IRuntimeError error);
    }
}
