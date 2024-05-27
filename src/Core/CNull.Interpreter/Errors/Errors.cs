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

    public record FunctionNotFoundError(string FunctionName, string ModuleName, Position Position) : ICompilationError
    {
        public string Message => $"The function: {FunctionName} was not found in module: {ModuleName}.";
    }

    public record ModuleNotFoundError(string ModuleName, Position Position) : ICompilationError
    {
        public string Message => $"Module: {ModuleName} was not found in the working directory.";
    }

    public record ModuleCompilationError(string ModuleName, Position Position) : ICompilationError
    {
        public string Message => $"Compilation of module: {ModuleName} has finished with errors.";
    }
}
