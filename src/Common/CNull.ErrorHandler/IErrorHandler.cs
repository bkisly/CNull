using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Events;
using CNull.ErrorHandler.Exceptions;

namespace CNull.ErrorHandler
{
    /// <summary>
    /// Provides an interface to raise errors and effectively redirect them to the user.
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// The queue of errors which occurred.
        /// </summary>
        IEnumerable<IError> Errors { get; }

        /// <summary>
        /// Event raised when an error occurred.
        /// </summary>
        event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// Raises an error which occurred during interacting with the source.
        /// </summary>
        /// <param name="error">Error to raise.</param>
        FatalErrorException RaiseSourceError(ISourceError error);

        /// <summary>
        /// Raises an error which occurred during static analysis of the code, which is not fatal.
        /// </summary>
        /// <param name="error">Error to raise.</param>
        void RaiseCompilationError(ICompilationError error);

        /// <summary>
        /// Raises an error which occurred during static analysis of the code, which is fatal and makes program execution uncontinuable.
        /// </summary>
        /// <param name="error"></param>
        FatalErrorException RaiseFatalCompilationError(ICompilationError error);

        /// <summary>
        /// Raises an error which occurred at runtime.
        /// </summary>
        /// <param name="error">Error to raise.</param>
        FatalErrorException RaiseRuntimeError(IRuntimeError error);
    }
}
