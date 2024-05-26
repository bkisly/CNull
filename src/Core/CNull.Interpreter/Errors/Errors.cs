using CNull.Common;
using CNull.ErrorHandler.Errors;

namespace CNull.Interpreter.Errors
{
    public record CircularDependencyError(Position Position) : ICompilationError
    {
        public string Message => "Circular dependency detected.";
    }

    public record FunctionRedefinitionError(string FunctionName, Position Position) : ICompilationError
    {
        public string Message => $"Function with name: {FunctionName} has already been defined.";
    }
}
