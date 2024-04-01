using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Events.Args;

namespace CNull.ErrorHandler
{
    public class ErrorHandler : IErrorHandler
    {
        public event EventHandler<ErrorOccurredEventArgs>? ErrorOccurred;

        public void RaiseSourceError(ISourceError error)
        {
            var messageHeader = $"C? initialization error: {nameof(error)}{Environment.NewLine}";
            OnErrorOccurred($"{messageHeader}{error.Message}");
        }

        public void RaiseCompilationError(ICompilationError error)
        {
            var messageHeader = $"C? error (line: {error.LineNumber}, column: {error.ColumnNumber}): {nameof(error)}{Environment.NewLine}";
            OnErrorOccurred($"{messageHeader}{error.Message}");
        }

        public void RaiseRuntimeError(IRuntimeError error)
        {
            var messageHeader = $"C? unhandled exception: {nameof(error)}{Environment.NewLine}";
            OnErrorOccurred($"{messageHeader}{error.Message}");
        }

        private void OnErrorOccurred(string message) => ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(message));
    }
}
