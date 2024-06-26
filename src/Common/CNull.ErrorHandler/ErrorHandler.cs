﻿using System.Text;
using CNull.Common.Configuration;
using CNull.Common.State;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Events;
using CNull.ErrorHandler.Exceptions;
using Microsoft.Extensions.Logging;

namespace CNull.ErrorHandler
{
    public class ErrorHandler(ILogger<IErrorHandler> logger, ICNullConfiguration config, IStateManager stateManager) : IErrorHandler
    {
        private readonly Queue<IError> _errors = new();
        public IEnumerable<IError> Errors => _errors;

        public event EventHandler<ErrorOccurredEventArgs>? ErrorOccurred;

        public FatalErrorException RaiseSourceError(ISourceError error)
        {
            RaiseError(error, stateManager.CurrentSourcePath,
                $"C? initialization error: {error.GetType().Name}{Environment.NewLine}");
            return FatalError();
        }

        public void RaiseCompilationError(ICompilationError error)
        {
            RaiseError(error, stateManager.CurrentSourcePath,
                $"C? error (line: {error.Position.LineNumber}, column: {error.Position.ColumnNumber}): {error.GetType().Name}{Environment.NewLine}");

            if (_errors.Count >= config.MaxErrorsCount)
                throw FatalError();
        }

        public FatalErrorException RaiseFatalCompilationError(ICompilationError error)
        {
            RaiseCompilationError(error);
            return FatalError();
        }

        public FatalErrorException RaiseSemanticError(ISemanticError error)
        {
            var lineInfo = error.LineNumber != null ? $" (line: {error.LineNumber})" : string.Empty;
            RaiseError(error, error.ModuleName,
                $"C? error{lineInfo}: {error.GetType().Name}{Environment.NewLine}");
            return FatalError();
        }

        public FatalErrorException RaiseRuntimeError(IRuntimeError error)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"C? unhandled exception (most recent call first): {error.Message}");

            foreach (var callStackRecord in error.CallStack)
            {
                messageBuilder.Append(
                    $"\tat {callStackRecord.CallingModule}.{callStackRecord.CallingFunction}");

                if (callStackRecord.CallingLineNumber > 0)
                    messageBuilder.Append($", line: {callStackRecord.CallingLineNumber}");

                messageBuilder.AppendLine();
            }

            RaiseException(error, messageBuilder.ToString());
            return FatalError();
        }

        private void RaiseError(IError error, string source, string message)
        {
            _errors.Enqueue(error);
            logger.LogError($"Error occurred ({error.GetType().Name}, source: {source}): {error.Message}");
            ErrorOccurred?.Invoke(this,
                new ErrorOccurredEventArgs(
                    $"{message}Source: {source}{Environment.NewLine}{error.Message}{Environment.NewLine}"));
        }

        private void RaiseException(IRuntimeError error, string message)
        {
            _errors.Enqueue(error);
            logger.LogError(message);
            ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(message));
        }

        private FatalErrorException FatalError()
        {
            const string message = "Fatal error occurred, unable to continue program execution.";
            logger.LogError(message);
            return new FatalErrorException(message);
        }
    }
}
