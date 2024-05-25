using CNull.Common.Configuration;
using CNull.Common.Mediators;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Events;
using CNull.ErrorHandler.Exceptions;
using Microsoft.Extensions.Logging;

namespace CNull.ErrorHandler
{
    public class ErrorHandler(ILogger<IErrorHandler> logger, ICNullConfiguration config, ICoreComponentsMediator mediator) : IErrorHandler
    {
        private readonly Queue<IError> _errors = new();
        public IEnumerable<IError> Errors => _errors;

        public event EventHandler<ErrorOccurredEventArgs>? ErrorOccurred;

        public void RaiseSourceError(ISourceError error)
        {
            RaiseError(error, $"C? initialization error: {error.GetType().Name}{Environment.NewLine}");
            throw FatalError();
        }

        public void RaiseCompilationError(ICompilationError error)
        {
            RaiseError(error,
                $"C? error (line: {error.Position.LineNumber}, column: {error.Position.ColumnNumber}): {error.GetType().Name}{Environment.NewLine}");

            if (_errors.Count >= config.MaxErrorsCount)
                throw FatalError();
        }

        public void RaiseRuntimeError(IRuntimeError error)
        {
            RaiseError(error, $"C? unhandled exception: {error.GetType().Name}{Environment.NewLine}");
        }

        private void RaiseError(IError error, string message)
        {
            _errors.Enqueue(error);

            var source = mediator.CurrentSourcePath;
            logger.LogError($"Error occurred ({error.GetType().Name}, source: {source}): {error.Message}");
            ErrorOccurred?.Invoke(this,
                new ErrorOccurredEventArgs(
                    $"{message}Source: {source}{Environment.NewLine}{error.Message}{Environment.NewLine}"));
        }

        private FatalErrorException FatalError()
        {
            const string message = "Fatal error occurred, unable to continue program execution.";
            logger.LogError(message);
            return new FatalErrorException(message);
        }
    }
}
