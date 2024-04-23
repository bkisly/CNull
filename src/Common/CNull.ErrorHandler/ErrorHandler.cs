using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Events.Args;
using CNull.ErrorHandler.Exceptions;

namespace CNull.ErrorHandler
{
    public class ErrorHandler : IErrorHandler
    {
        public event EventHandler<ErrorOccurredEventArgs>? ErrorOccurred;

        public void RaiseSourceError(ISourceError error)
        {
            var messageHeader = $"C? initialization error: {error.GetType().Name}{Environment.NewLine}";
            OnErrorOccurred($"{messageHeader}{error.Message}");
            FatalError();
        }

        public void RaiseCompilationError(ICompilationError error)
        {
            var messageHeader = $"C? error (line: {error.Position.LineNumber}, column: {error.Position.ColumnNumber}): {error.GetType().Name}{Environment.NewLine}";
            OnErrorOccurred($"{messageHeader}{error.Message}");
        }

        public void RaiseRuntimeError(IRuntimeError error)
        {
            var messageHeader = $"C? unhandled exception: {error.GetType().Name}{Environment.NewLine}";
            OnErrorOccurred($"{messageHeader}{error.Message}");
        }

        private void OnErrorOccurred(string message) => ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(message));

        private static void FatalError()
        {
            throw new FatalErrorException("Fatal error occurred, unable to continue program execution.");
        }
    }
}
