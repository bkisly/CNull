using CNull.Common;
using CNull.ErrorHandler.Errors;

namespace CNull.Interpreter.Errors
{
    public record UnhandledExceptionError(string Exception, IEnumerable<CallStackRecord> CallStack) : IRuntimeError
    {
        public string Message { get; } = Exception;
    }

    public record StackOverflowError(IEnumerable<CallStackRecord> CallStack) : IRuntimeError
    {
        public string Message => "Stack overflow. Showing last 100 call stack entries.";
    }
}
